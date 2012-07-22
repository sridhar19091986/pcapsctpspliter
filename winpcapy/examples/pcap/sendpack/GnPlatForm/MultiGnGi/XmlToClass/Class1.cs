using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiGnGi.XmlToClass
{
    class Class1
    {
       //2012.7.22

        public void testxsd()
        {
            //
            //大型文件用碎片，分片，
            //例如，tshark -> pmdl(xml)  ->xsd -> serializer -> class -> mongodb
            //mongodb mapreduce/linq->list

            System.IO.StreamReader str = new System.IO.StreamReader("webSearch.xml");
            System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(NewDataSet));
            NewDataSet res = (NewDataSet)xSerializer.Deserialize(str);
            foreach (var r in res.packet)
            {
                Console.WriteLine(r.packet_Id);
                Console.WriteLine(r.pdml_Id);
                Console.WriteLine();
            }
            str.Close();

            Console.ReadLine();
        }

    }
}
