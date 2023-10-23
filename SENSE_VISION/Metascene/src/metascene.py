#!/usr/bin/env python
import rospy
from std_msgs.msg import String

import math
import json

from metascene_class import Metascene, Subject, FaceAnalysis, Environment, gestureType
from tools import *
from timer_reset import *

pub = None
DeltaErrorX=10
metascene=Metascene()


def cleanSubject():
    global metascene
    try:
        metascene.subjects=[]
    except ex:
        rospy.loginfo(rospy.get_caller_id() + ex.message)

timerResetSubject = TimerReset(1.0, cleanSubject, args=[], kwargs={})

def callbackBodies(data, test=False):
    global metascene, timerResetSubject

    timerResetSubject.reset(1)

    if not test:
        #rospy.loginfo(rospy.get_caller_id() + 'I bodies %s', data.data)
        infoSubjects = json.loads(data.data)
    else:
        infoSubjects = data

    #elimino soggetti non presenti 
    for subject in infoSubjects:
        metascene.subjects = [x for x in metascene.subjects if x.trackingId == subject["TrackingId"]]

    for subject in infoSubjects:
        s = Subject(subject)
        g = gesture(subject["Joints"])

        s.gesture=g
        s.setSpeakProb(metascene.environment.soundAngle)


        indexes = metascene.findIndexSubject( subject["TrackingId"])

        if(indexes):   
            metascene.subject[indexes[0]].update(subject)
            metascene.subject[indexes[0]].gesture = g
            metascene.subjects[indexes[0]].setSpeakProb(metascene.environment.soundAngle)
        else:
            metascene.subjects.append(s)
    
    metascene.environment.numberSubject = len(metascene.subjects);
     
def callbackAudioInfo(data, test=False):
    global metascene

    if not test:
       # rospy.loginfo(rospy.get_caller_id() + 'I audio %s', data.data)
        audioData=json.loads(data.data)

    else:
        audioData=data

    metascene.environment.soundAngle = audioData["beamAngle"] * 180.0 / math.pi;
    metascene.environment.soundEstimatedX = math.tan(audioData["beamAngle"]);
    metascene.environment.soundDecibel =audioData["soundDecibel"];

def callbackShore(data, test=False):

    timerResetSubject.reset(1)

    if not test:
        rospy.loginfo(rospy.get_caller_id() + 'I shore %s', data.data)
        shores = json.loads(data.data)
    else:
        shores = data



    for shore in shores:

        #region check corrispondenza subject
        present = False

        for subject in metascene.subjects:

            if  subject.headPosition is not None and subject.headPosition["Z"]>0:
                #calcolo il tunto medio degli occhi
                middleEyes = ((shore["Eyes"]["Left"]["X"] + shore["Eyes"]["Right"]["X"]) / 2, shore["Eyes"]["Left"]["Y"]);
                shorePointX = MetersToPoint(subject.headPosition)[0];
                ErrorX = shorePointX - middleEyes[0]; #Errore tra la posizione di shore e del kinect
        
                if math.fabs(ErrorX) < DeltaErrorX:
                    subject.faceAnalysis.update(shore)
                    present = True

        if not present:
            s= Subject(None)
            s.faceAnalysis = FaceAnalysis(shore)
            metascene.subjects.append(s)

    metascene.environment.numberSubject = len(metascene.subjects);


def listener():
    global metascene
    # In ROS, nodes are uniquely named. If two nodes with the same
    # name are launched, the previous one is kicked off. The
    # anonymous=True flag means that rospy will choose a unique
    # name for our 'listener' node so that multiple listeners can
    # run simultaneously.
    rospy.init_node('metascene', anonymous=True)

    rospy.Subscriber('/kinect/bodies', String, callbackBodies)
    rospy.Subscriber('/kinect/audio/info', String, callbackAudioInfo)
    rospy.Subscriber('/shore', String, callbackShore)

def talker():
    global pub, metascene

    pub = rospy.Publisher('metascene', String, queue_size=10)
    rate = rospy.Rate(10) # 10hz

    while not rospy.is_shutdown():
        hello_str = json.dumps(metascene,default=lambda x: x.__dict__)
        pub.publish(hello_str)
        rate.sleep()


    

if __name__ == '__main__':
    listener()
    timerResetSubject.start() #- to start the timer

    try:
        talker()
    except rospy.ROSInterruptException:
        pass

    
    rospy.spin()



