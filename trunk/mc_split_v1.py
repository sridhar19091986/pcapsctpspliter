
# $Id: mc_split.py,v 1.0 2010/11/15 14:10:10 WeiHongPing $
# Authors: WeiHongPing

import sys
import string
import struct
import dpkt
from dpkt.ip import IP  
import pcapy
from pcapy import *

class C(object):
      link=''
      ip=''
      sctphdr=''
      lastsctp=''
      dumpnewstctp=''
      sctp=''
      totallen=0
      sctpNum = 0
      sctps=[]    
      #iphdr=""
      #hdr=""
      #newip=""
      #newsctp=""

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
      #protocol stack data payload
      #
      eth = dpkt.ethernet.Ethernet(data)
      C.hdr=hdr
      #print repr(hdr)
      #
      C.totallen=len(data)
      #print C.totallen
      #C.hdr=''
      ethhdr=eth.__str__()
      #link
      C.link=ethhdr[:14]
      ip=eth.data
      #ip.src
      #ip
      #
      C.ip=ethhdr[14:34]

      sctp=ip.data
      C.sctp=sctp.__str__()
      #sctp
      C.sctphdr=C.sctp[:(12+0)]
      i=0
      C.sctpNum = 0
      C.sctps=[]
      C.lastsctp=''
      while i+8 < len(C.sctp):
            if C.sctp[i:i+8]== itag:
               C.sctps.append(i)
               C.sctpNum +=1
                  #print C.sctpNum
                  #print len(C.sctps)
            i+=1
      while not Terminated :
          if  C.sctpNum-1 > 0 :
              #n->
              C.sctpNum -=1              
              C.lastsctp=C.sctp[C.sctps[C.sctpNum]-12:]
              #C.newip.len=len(C.lastsctp)+20
              #print repr(C.lastsctp)
              C.dumpnewsctp=C.link+C.ip+C.sctphdr+C.lastsctp
              while len(C.dumpnewsctp)<C.totallen:
                    C.dumpnewsctp +=editcaptag
              dumper.dump(C.hdr,C.dumpnewsctp)
              C.sctps.remove(C.sctps[C.sctpNum])
              #0->n
              C.sctp=C.sctp[:-len(C.lastsctp)]
              Terminated = False            
          else:
              #C.newip.len=len(C.sctp)+20
              C.dumpnewsctp=C.link+C.ip+C.sctp
              while len(C.dumpnewsctp)<C.totallen:
                    C.dumpnewsctp +=editcaptag
              #print repr(C.sctp)
              dumper.dump(C.hdr,C.dumpnewsctp)
              Terminated = True

def main(filename):
  p = pcapy.open_offline(filename)
  print "Reading from %s: linktype=%d" % (filename, p.datalink())
  Decoder(p).start()
if __name__ == '__main__':
    if len(sys.argv) <= 1:
        print "Usage: %s <filename>" % sys.argv[0]
        sys.exit(1)
    main(sys.argv[1])
