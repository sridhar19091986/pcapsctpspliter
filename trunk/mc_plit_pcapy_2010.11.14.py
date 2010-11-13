

import sys
import string
import struct
import dpkt
from dpkt.ip import IP  
import pcapy
from pcapy import *

class C(object):
      sctpNum = 0
      sctps=[]
      sctp=''
      iphdr=''
      lastsctp=''
      hdr=''
      newip=''
      newsctp=''
      sctphdr=''
      totallen=''
      dumpnewstctp=''
      unpacksctp='\x03\x00\x00\x10\x0a\xd0\xf0\xb3\x00\x00\xff\xff\x00\x00\x00\x00'
      packsctp='\x0b\x59\x0b\x59\x00\x00\xfa\xbc\x8a\x30\xbf\x0f'

class Decoder:
  def __init__(self, pcapObj):
      global selfpcap
      selfpcap = pcapObj
      newfile = 'mm.pcap'
      global dumper
      dumper = selfpcap.dump_open(newfile)
      global itag
      itag='\x00\x00\x00\x03\x01\x00\x01\x01'
      global editcaptag
      editcaptag='\x00'
      C.sctpNum=0
     
  def start(self):  
      selfpcap.loop(0, self.packetHandler)

  def packetHandler(self, hdr, data):
      Terminated = False
      eth = dpkt.ethernet.Ethernet(data)
      C.hdr=hdr
      C.sctp=eth.__str__()
      C.totallen=len(data)
      print C.totallen
      dumper.dump(hdr,data)
      i=0
      C.sctpNum = 0
      C.sctps=[]
      C.lastsctp=''
      while i+8 < len(C.sctp):
          if C.sctp[i:i+8]== itag:
                  C.sctps.append(i)
                  C.sctpNum +=1
          i+=1
      while not Terminated :
          C.sctpNum -=1
          #偏移n到末尾
          C.lastsctp=C.sctp[C.sctps[C.sctpNum]-12:]
          #0开始不包含末尾n个元素
          C.sctp=C.sctp[:-len(C.lastsctp)]
          while len(C.sctp)<C.totallen:
                C.sctp +=editcaptag
          dumper.dump(C.hdr,C.sctp)
          C.sctps.remove(C.sctps[C.sctpNum])        
          if  C.sctpNum-1 > 0 :
              Terminated = False            
          else:
              Terminated = True

def main(filename):
  p = pcapy.open_offline(filename)
  print "Reading from %s: linktype=%d" % (filename, p.datalink())
  Decoder(p).start()
if __name__ == '__main__':
  if len(sys.argv) <= 1:
      main('bbbb.pcap')
  else:
      print "Usage: %s <filename>" % sys.argv[0]
      #sys.exit(1)
  #main(sys.argv[1])  
