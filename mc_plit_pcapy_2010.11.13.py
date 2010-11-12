

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
      sctp=""
      iphdr=""
      lastsctp=""
      hdr=""
      newip=""
      newsctp=""
      sctphdr=""
      totallen=""
      dumpnewstctp=""

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
      print repr(hdr)
      C.totallen=len(data)
      print C.totallen
      #C.hdr=''
      ethhdr=eth.__str__()
      C.iphdr=ethhdr[:14]
      ip=eth.data
      #ip.src
      C.newip = IP(src=ip.src, dst=ip.dst, p=dpkt.ip.IP_PROTO_SCTP)
      sctp=ip.data
      C.sctp=sctp.__str__()
      #C.sctphdr=C.sctp[:(12+16+16+4)]
      #C.sctphdr=C.sctp[:12]
      C.sctphdr=C.sctp[:28]
      while not Terminated :
          i=0
          C.sctpNum = 0
          C.sctps=[]
          C.lastsctp=""
          while i+8 < len(C.sctp):
              if C.sctp[i:i+8]== itag:
                  C.sctps.append(i)
                  C.sctpNum +=1
                  #print C.sctpNum
                  #print len(C.sctps)
              i+=1
          if  C.sctpNum-1 > 0 :
              #偏移n到末尾
              C.sctpNum -=1              
              C.lastsctp=C.sctp[C.sctps[C.sctpNum]-12:]
              C.newip.len=len(C.lastsctp)+20
              #print repr(C.lastsctp)
              C.dumpnewsctp=C.iphdr+C.newip.__str__()+C.sctphdr.__str__()+C.lastsctp.__str__()
              while len(C.dumpnewsctp)<C.totallen:
                    C.dumpnewsctp +=editcaptag
              dumper.dump(C.hdr,C.dumpnewsctp)
              C.sctps.remove(C.sctps[C.sctpNum])
              #0开始不包含末尾n个元素
              C.sctp=C.sctp[:-len(C.lastsctp)]
              Terminated = False            
          else:
              C.newip.len=len(C.sctp)+20
              C.dumpnewsctp=C.iphdr+C.newip.__str__()+C.sctp
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
      main('bbbb.pcap')
  else:
      print "Usage: %s <filename>" % sys.argv[0]
      #sys.exit(1)
  #main(sys.argv[1])  
