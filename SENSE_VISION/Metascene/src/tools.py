#!/usr/bin/env python
import math
from metascene_class import  gestureType


def MetersToPoint(p):
    Xmax = p["Z"] * math.tan((57/180)* 3.14)
    xp = ((p["X"]/Xmax)/2)+0.5

    Ymax = p["Z"] * math.tan((43/180)* 3.14)
    yp = ((p["Y"]/Ymax)/2)+0.5

    return (xp,yp)

def DifferencePoints( pos1, pos2):
    deltaX = pos1["X"] - pos2["X"]
    deltaY = pos1["Y"] - pos2["Y"]
    deltaZ = pos1["Z"] - pos2["Z"]


    return math.sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ*deltaZ))
   
def gesture( data):
gestureType = ['none', 'raiseHand', 'YEAH', 'think','armsCrossed','pray', 'hand' ]


    if data["Head"]["Position"]["Z"] != 0 and data["SpineMid"]["Position"]["Z"] != 0:
       
        treshold = (data["Head"]["Position"]["Y"] - data["SpineShoulder"]["Position"]["Y"]) * 0.75

        if (DifferencePoints(data["Head"]["Position"], data["HandTipRight"]["Position"]) < 0.20)  ^ (DifferencePoints(data["Head"]["Position"], data["HandTipLeft"]["Position"]) < 0.20):
            return gestureType[3] #think

        elif (((data["HandRight"]["Position"]["Y"] > treshold) ^ (data["HandLeft"]["Position"]["Y"] > treshold))
            and (data["Head"]["Position"]["Y"] - data["SpineMid"]["Position"]["Y"]) > 0.05):
            
            return gestureType[1]#raisehand

        elif (DifferencePoints(data["WristLeft"]["Position"], data["ElbowRight"]["Position"]) < 0.20 
            and DifferencePoints(data["WristRight"]["Position"], data["ElbowLeft"]["Position"]) < 0.20):
            
            return gestureType[4]#armcrossed

        elif DifferencePoints(data["WristLeft"]["Position"],data["WristRight"]["Position"]) < 0.20:
            return gestureType[5]#pray

        elif DifferencePoints(data["HandTipRight"]["Position"], data["ThumbRight"]["Position"]) < 0.05:
            return gestureType[0] #none #gestureType[6] #hand
                            
        elif DifferencePoints(data["HandRight"]["Position"], data["HandLeft"]["Position"]) > 1.03:
            return gestureType[7]

        elif (((data["HandRight"]["Position"]["Y"]  > treshold) 
            and (data["HandLeft"]["Position"]["Y"] > treshold) 
            and (data["HandRight"]["Position"]["Y"]  - data["HandLeft"]["Position"]["Y"]) < 0.15) 
            and (data["Head"]["Position"]["Y"] - data["SpineMid"]["Position"]["Y"]) > 0.05):

            return gestureType[2] #Yeah
    else:
        return gestureType[0] #none