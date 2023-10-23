#!/usr/bin/env python
import math


gestureType = ['none', 'raiseHand', 'YEAH', 'think','armsCrossed','pray', 'hand' ]

class Metascene(object):
    def __init__(self):
        super(Metascene, self).__init__()
        self.subjects = []
        self.environment = Environment()

    def findIndexSubject(self, trackingId):
        for i,x in enumerate(self.subjects):
            if x.trackingId == trackingId:
                return i

    def __str__(self):
        return str(self.__class__) + ": " + str(self.__dict__)

class Subject(object):
    def __init__(self, data):
        super(Subject, self).__init__()
        self.id = 0
        self.name = ""
        self.trackingId = data["TrackingId"] if data is not None else 0
        self.clippedEdges = data["ClippedEdges"] if data is not None else 0
        self.handRightConfidence = data["HandRightConfidence"] if data is not None else 0
        self.handRightState= data["HandRightState"] if data is not None else 0
        self.handLeftConfidence= data["HandLeftConfidence"] if data is not None else 0
        self.handLeftState= data["HandLeftState"] if data is not None else 0
        self.headPosition=data["Joints"]["Head"]["Position"] if data is not None else {"X":0, "Y":0,"Z": 0}

        self.angle = round((math.atan((self.headPosition["X"] / self.headPosition["Z"])) * (180 / (math.pi))), 2) if data is not None else None
        self.speak_prob = 0
        self.gesture = gestureType[0]
        self.faceAnalysis =None

    def update(self, data):
        self.clippedEdges = data["ClippedEdges"]
        self.handRightConfidence = data["HandRightConfidence"]
        self.handRightState= data["HandRightState"]
        self.handLeftConfidence= data["HandLeftConfidence"]
        self.handLeftState= data["HandLeftState"]

    def setSpeakProb(self, soundAngle):
        if self.angle is not None and soundAngle is not None:
            self.speak_prob = math.fabs(math.fabs(self.angle - soundAngle) - 70) / 70;


    def __eq__(self, other):
        """To implement 'in' operator"""
        # Comparing with another Test object
        if isinstance(other, Subject):
            return self.trackingId == other.trackingId

    def __str__(self):
        return str(self.__class__) + ": " + str(self.__dict__)


class FaceAnalysis(object):
    def __init__(self, data):
        super(FaceAnalysis, self).__init__()
        self.age = data["Age"]
        self.ageDeviation = data["Age_deviation"]
        self.gender = data["Gender"]
        self.uptime =  data["Uptime"]
        self.happinessRatio=data["Happiness_ratio"]
        self.surpriseRatio= data["Surprise_ratio"]
        self.angerRatio=data["Anger_ratio"]
        self.sadnessRatio= data["Sadness_ratio"]

    def update(self, data):
        self.age = data["Age"]
        self.ageDeviation = data["Age_deviation"]
        self.gender = data["Gender"]
        self.uptime =  data["Uptime"]
        self.happinessRatio=data["Happiness_ratio"]
        self.surpriseRatio= data["Surprise_ratio"]
        self.angerRatio=data["Anger_ratio"]
        self.sadnessRatio= data["Sadness_ratio"]

    def __str__(self):
        return str(self.__class__) + ": " + str(self.__dict__)


class Environment(object):
    def __init__(self):
        super(Environment, self).__init__()
        self.soundAngle=None
        self.soundEstimatedX = None
        self.recognizedWord = None
        self.numberSubject = 0
        self.soundDecibel = 0
        self.saliency =[]

    def __str__(self):
        return str(self.__class__) + ": " + str(self.__dict__)

