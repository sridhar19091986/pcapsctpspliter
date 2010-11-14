import getopt
import sys
import re
import string
import os
import tempfile

### User settings
#################################################
# Absolute path to text2pcap (usually in the
# ethereal directory
# Example path: C:\Projects\trunk\text2pcap.exe
TEXT2PCAP_PATH = os.path.join('C:\\', 'Projects', 'trunk', 'text2pcap.exe')

# Temp file directory, write access required. All
# temporary files are removed automatically when
# tektronix2pcap exits
DEFAULT_TEMP_DIR = tempfile.gettempdir()
  # Default temp dir is set by the os
TEMP_DIR = DEFAULT_TEMP_DIR
#################################################

def tektronix2pcap(inFileName, outFileName):
  OFFSET_EXPR = '^[0-9a-fA-F]{3,}'

  inFile = open(inFileName, 'r')
  tempFileName = tempfile.mktemp(dir=TEMP_DIR)
  tempFile = open(tempFileName, 'w')

  schemeOffset = re.compile(OFFSET_EXPR)
  
  rows = inFile.readlines()
  
  for row in rows:
    offsetRes = schemeOffset.search(row)
    if offsetRes:
      tempFile.write(string.replace(row, ': ', ''))
     
  inFile.close()
  tempFile.close()

  arg = TEXT2PCAP_PATH + ' -q ' + tempFileName + ' ' +  outFileName 
  os.system(arg)

#################################################

def printUsage():
  sys.stderr.write("\nUsage: tektronix2pcap <infile> <outfile>\n")
  
#################################################
optlist, args = getopt.getopt(sys.argv[1:], 'd')

if len(args) == 2:
  tektronix2pcap(args[0], args[1])
else:
  printUsage()
#################################################









