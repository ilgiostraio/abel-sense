#!/usr/bin/env python

from threading import Thread, Event, Timer
import time



class TimerReset(Thread):
    """Call a function after a specified number of seconds:
    t = TimerReset(30.0, f, args=[], kwargs={})
    t.start() - to start the timer
    t.reset() - to reset the timer
    t.cancel() # stop the timer's action if it's still waiting
    """

    def __init__(self, interval, function, args=[], kwargs={}):
        Thread.__init__(self)
        self.interval = interval
        self.function = function
        self.args = args
        self.kwargs = kwargs
        self.finished = Event()
        self.resetted = True
        self.cancel= False

    def cancel(self):
        """Stop the timer if it hasn't finished yet"""
        self.finished.set()
        self.cancel=True

    def run(self):
        #print("Time: %s - timer running..." % time.asctime())

        while not self.cancel:
            while self.resetted:
          #      print("Time: %s - timer waiting for timeout in %.2f..." % (time.asctime(), self.interval))
                self.resetted = False
                self.finished.wait(self.interval)

            if not self.finished.isSet():
                self.function(*self.args, **self.kwargs)
                self.resetted = True

            self.finished.set()
            self.finished.clear()

         #   print("Time: %s - timer restat!" % time.asctime())

        #print("Time: %s - timer finished!" % time.asctime())

    def reset(self, interval=None):
        """ Reset the timer """
        
        if interval:
            #print("Time: %s - timer resetting to %.2f..." % (time.asctime(), interval))
            self.interval = interval
        else:
            print("Time: %s - timer resetting..." % time.asctime())

        self.resetted = True
        self.finished.set()
        self.finished.clear()
