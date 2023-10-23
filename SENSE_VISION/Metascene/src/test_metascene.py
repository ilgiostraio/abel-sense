#!/usr/bin/env python
import json

from metascene  import *
from data_test import *


callbackAudioInfo(audioinfo, True)
callbackBodies(bodies, True)
callbackShore(shoreTest, True)

print(json.dumps(metascene,sort_keys=True,indent=4,default=lambda x: x.__dict__))


