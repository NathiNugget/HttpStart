using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Collections;

namespace HttpStart
{
    public class Server
    {
        private const int PORT = 727;
        private List<Person> _personList = new List<Person>();
        private static readonly string RootCatalog = "C:\\Users\\natha\\source\\repos\\HttpStart\\HttpStart";

        public void Start()
        {
            // definerer server med port nummer
            TcpListener server = new TcpListener(PORT);
            server.Start();
            Console.WriteLine("Server startet på port " + PORT);

            while (true)
            {
                // venter på en klient 
                TcpClient socket = server.AcceptTcpClient();
                Task.Run(() =>
                {
                    DoOneClient(socket);
                });
            }

        }

        private void DoOneClient(TcpClient socket)
        {
            Console.WriteLine($"Min egen (IP, port) = {socket.Client.LocalEndPoint}");
            Console.WriteLine($"Accepteret client (IP, port) = {socket.Client.RemoteEndPoint}");


            // åbner for tekst strenge
            StreamReader sr = new StreamReader(socket.GetStream());
            StreamWriter sw = new StreamWriter(socket.GetStream());

            while (socket.Connected)
            {



                // læser linje fra nettet
                string l = sr.ReadLine();

                /*
                // hvis tælle ord
                Console.WriteLine("Modtaget: " + l);

                // hvis der skal summers tal: 
                if (l.ToUpper().Contains("ADD"))
                {
                    string[] _numbersToAdd = l.Split(' ');
                    int sum = 0;
                    for (int i = 1; i < _numbersToAdd.Length; i++)
                    {
                        sum += int.Parse(_numbersToAdd[i]);
                    }

                    sw.WriteLine($"Sum of numbers: {sum}");


                }
                else if (l.ToUpper().Contains("SUB"))
                {
                    string[] _numbersToSub = l.Split(' ');
                    int sum = int.Parse(_numbersToSub[1]) - int.Parse(_numbersToSub[2]);
                    Console.WriteLine($"Calculation: {_numbersToSub[1]} - {_numbersToSub[2]}: \nSum: {sum}");
                }

                else if (l.ToUpper().Contains("MUL"))
                {
                    string[] _numbersToMul = l.Split(' ');
                    int sum = int.Parse(_numbersToMul[1]) * int.Parse(_numbersToMul[2]);
                    Console.WriteLine($"Calculation: {_numbersToMul[1]} * {_numbersToMul[2]}: \nSum: {sum}");
                }


                else
                {
                    // skriver linje tilbage - stadig ekko
                    sw.WriteLine($"Antal ord: {l.ToUpper().Split(' ').Length}: {l.ToUpper()}");
                    PrintOnEachLine(sw, l);
                }
                */


                //////////////////////////////////////////////////////////////////////////////////
                /// If to try CRUD
                /// 
                /// 
                /// 
                /// 
                /// 
                /// 
                /// 
                /// 
                /// 
                string[] line = { "", "" };
                if (l != null)
                {
                    line = l.Split(' ');
                    if (line.Length > 1)
                    {
                        if (l.Contains("GET"))
                        {
                            string URI = line[1].Replace("/", "\\");
                            int imageLen = 0;
                            string _tofind = RootCatalog + URI;
                            using (FileStream fs = File.OpenRead(_tofind))
                            {
                                imageLen = (int)fs.Length;
                            }
                            StringBuilder sb = new StringBuilder();
                            sw.Write($"HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nConnection: close\r\n\r\n");



                            Console.WriteLine(_tofind);

                            if (File.Exists(_tofind))
                            {

                                using (StreamReader streamReader = new StreamReader(_tofind))
                                {
                                    while (!streamReader.EndOfStream)
                                    {

                                        sw.WriteLine(streamReader.ReadLine());

                                    }
                                    sw.Flush();
                                }
                                

                                



                            }
                            string navn = line[1];
                            if (line.Length > 3)
                            {
                                string addresse = line[2];
                                string mobil = line[3];
                                if (l.ToUpper().Contains("CREATE"))
                                {
                                    AddPerson(navn, addresse, mobil, sw);
                                }

                                else if (l.ToUpper().Contains("UPDATE"))
                                {
                                    navn = line[1];
                                    string nnavn = line[2];
                                    string naddresse = line[3];
                                    string nmobil = line[4];
                                    UpdatePerson(navn, nnavn, naddresse, nmobil, sw);
                                }

                            }



                            else if (l.ToUpper().Contains("DELETE")) DeletePerson(navn, sw);
                        }
                        else if (l.ToUpper().Contains("READ"))
                        {
                            ReadList(sw);
                        }
                        else if (l.ToUpper().Contains("EXIT") || l.ToUpper().Contains("STOP"))
                        {
                            sw.WriteLine("You have been disconnected from the server");
                            sw.Flush();
                            socket.Close();
                            break;
                        }
                        sw.Flush();
                        socket.Close();
                    }
                    else
                    {
                        sw.Flush();
                        socket.Close();
                        break;


                    }






                }





                }

                Console.WriteLine("Client disconnected");
            }

            private void PrintOnEachLine(StreamWriter sw, string l)
            {
                foreach (string s in l.Split(' '))
                {
                    sw.WriteLine(s);
                }
            }

            private void DeletePerson(string navn, StreamWriter sw)
            {
                Person? person = _personList.Find(p => p.Navn.Equals(navn));
                if (person != null)
                {
                    _personList.Remove(person);
                    sw.WriteLine($"Person slettet: {person}");
                }
                else sw.WriteLine("Noget gik galt :(");

            }

            private void AddPerson(string navn, string addresse, string mobil, StreamWriter sw)
            {
                if (navn != null && addresse != null && mobil != null)
                {
                    Person p = new Person(navn, addresse, mobil);
                    _personList.Add(p);
                    sw.WriteLine($"Person tilføjet: {p}");
                }
                else sw.WriteLine($"Noget gik galt :(");
            }

            private void UpdatePerson(string navn, string nnavn, string naddresse, string nmobil, StreamWriter sw)
            {
                if (navn != null && nnavn != null && naddresse != null && nmobil != null)
                {
                    Person p = _personList.Find(p => p.Navn.Equals(navn))!;
                    if (p != null)
                    {
                        int idx = _personList.IndexOf(p);
                        p.Addresse = naddresse;
                        p.Navn = nnavn;
                        p.Mobil = nmobil;
                        _personList[idx] = p;
                        sw.WriteLine($"Person opdateret: {navn}");
                    }
                }
                else sw.WriteLine("Noget gik galt :(");
            }

            private void ReadList(StreamWriter sw)
            {
                lock (_personList)
                {
                    foreach (Person p in _personList)
                    {

                        sw.WriteLine(p);
                        sw.Flush();
                    }
                }


            }



        }
    }
