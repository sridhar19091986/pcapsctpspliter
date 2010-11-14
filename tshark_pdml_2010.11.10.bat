tshark -r 1.pcap -R "gsm_a.bssmap_msgtype==Paging" -E occurrence=a -E separator=, > aa.csv
