using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Net;
using System.Net.Sockets;



namespace network
{
    public partial class Form1 : Form
    {
        int[] valuesForCity;
        
        private Socket client;
        int opponentScore;
        List<int> randomList;
        List<int> myCity;
        List<int> opponentCity;
        Random a = new Random();
        string  dataTosend;
        string receivedData;
        int myScore;
        int Round;
        int GoldenFlagRound;
        int GoldenFlag;
        private byte[] data = new byte[1024];
        private int size = 1024;
        private Socket server;
        Boolean start; //this makes client number1 player and server number2


        


        public Form1()
        {
            InitializeComponent();

            Round = 0;
            UseWaitCursor = true;
            opponentScore = 0;
            myScore = 0;
            GoldenFlag = 999;
            GoldenFlagRound = a.Next(6, 11);
            start = true;
            //FOR THREAD ERRORS
            Control.CheckForIllegalCrossThreadCalls = false;
           
            randomList = new List<int>();
            myCity = new List<int>();
            opponentCity = new List<int>();

            //EVERY NUMBER REPRESENTS 1 CITY
            //ALL CITIES IN "ONE ARRAY" FOR CHECKING OPERATION
            //IMPORTANT: I AM USING 0 1 2 CONTROL, WHAT DOES IT MEAN?
            //IT MEANS IF CITY IS MINE THEN I WILL SET THAT CITY'S INDEX 0
            //IF ENEMY HAVE THAT CITY  I WILL SET THAT CITY'S INDEX 1
            //IF NOBODY HAVE THAT CITY THEN IT MUST BE 2
            int i = 0;
            valuesForCity = new int[28];
            for (i = 0; i < 28; i++)
            {
                valuesForCity[i] = 2;
            }
            Console.WriteLine(valuesForCity.Length);
            int MyNumber = 0;
            while (randomList.Count < 10)//SELECTING UNIQUE 10 CITIES
            {

                MyNumber = a.Next(0, valuesForCity.Length); 
                if (!randomList.Contains(MyNumber))
                {
                    randomList.Add(MyNumber);
                }
            }
             i = 0;

            //DISTRUBUTE CITIES (seperato into opponentcity or mycity)
            foreach (var rand in randomList)
            {
                if (i <= 4)
                {
                    myCity.Add(rand);
                }
                if (i > 4)
                {
                    opponentCity.Add(rand);
                }
                i++;
            }
            Console.WriteLine("mycity:");
            foreach (var city in myCity)
            {
                Console.WriteLine(city);

            }


            Console.WriteLine("opponent:");
            foreach (var city in opponentCity)
            {
                Console.WriteLine(city);
            }

            //DETERMINATION OF MY CITIES
            for (int j = 0; j < myCity.Count; j++)
            {
                valuesForCity[myCity[j]] = 0;


            }
            //DETERMINATION OF OPPONENT CITIES
            for (int j = 0; j < opponentCity.Count; j++)
            {
                valuesForCity[opponentCity[j]] = 1;


            }

            disableAll();

           //CONNECTION
            server = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9050);
            server.Bind(iep);
            server.Listen(5);
          
            server.BeginAccept(new AsyncCallback(AcceptConn), server);

        }

        void AcceptConn(IAsyncResult iar)
        {
           
            Socket oldserver = (Socket)iar.AsyncState;
             client = oldserver.EndAccept(iar);

            //ENCODING 10 CITIES AS A STRING TO SEND CLIENT
            string str = "";
            string str2 = "";
            int i = 0;
            foreach (var rand in randomList)
            {
                if (i <= 4)
                {
                    str += rand.ToString() + ',';
                }
                if (i > 4)
                {
                    str2 += rand.ToString() + ',';
                }
                i++;
            }
  
            Console.WriteLine("str");
            Console.WriteLine(str);
            Console.WriteLine("str2");
            Console.WriteLine(str2);

            //ENCODE
            str = "/"+str.Substring(0, str.Length - 1);
            str2 = str2.Substring(0, str2.Length - 1);
            dataTosend = str2+str;

            Console.WriteLine(str);
            Console.WriteLine(str2);
            Console.WriteLine(dataTosend);



            byte[] message1 = Encoding.ASCII.GetBytes(dataTosend);
            client.BeginSend(message1, 0, message1.Length, SocketFlags.None,
                        new AsyncCallback(SendData), client);
           
           
        }

        void SendData(IAsyncResult iar)
        {
            
             client = (Socket)iar.AsyncState;
            int sent = client.EndSend(iar);
            client.BeginReceive(data, 0, size, SocketFlags.None,
                        new AsyncCallback(ReceiveData), client);
        }
        

        void ReceiveData(IAsyncResult iar)
        {
            
            client = (Socket)iar.AsyncState;
            int recv = client.EndReceive(iar);
            if (recv == 0)
            {
                client.Close();
                Console.WriteLine( "Waiting for client...");
                server.BeginAccept(new AsyncCallback(AcceptConn), server);
                return;
            }
            
            receivedData = Encoding.ASCII.GetString(data, 0, recv);
            UseWaitCursor = false;
            enableAll();
            Round++;
            Console.WriteLine("Round {0}", Round);
            Console.WriteLine("random round {0}", GoldenFlagRound);
            if (Round == GoldenFlagRound)
            {
                status.Text = "GOLDEN FLAG ROUND";
                foreach (var city in valuesForCity)
                {
                    GoldenFlag = a.Next(0, valuesForCity.Length);
                    if (valuesForCity[GoldenFlag] == 2)
                    {
                        if (GoldenFlag == 0 && button0.Visible)
                        {
                            Console.WriteLine("button 0 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 1 && button1.Visible)
                        {
                            Console.WriteLine("button 1 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 2 && button2.Visible)
                        {
                            Console.WriteLine("button 2 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 3 && button3.Visible)
                        {
                            Console.WriteLine("button 3 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 4 && button4.Visible)
                        {
                            Console.WriteLine("button 4 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 5 && button5.Visible)
                        {
                            Console.WriteLine("button 5 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 6 && button6.Visible)
                        {
                            Console.WriteLine("button 6 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 7 && button7.Visible)
                        {
                            Console.WriteLine("button 7 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 8 && button8.Visible)
                        {
                            Console.WriteLine("button 8 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 9 && button9.Visible)
                        {
                            Console.WriteLine("button 9 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 10 && button10.Visible)
                        {
                            Console.WriteLine("button 10 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 11 && button11.Visible)
                        {
                            Console.WriteLine("button 11 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 12 && button12.Visible)
                        {
                            Console.WriteLine("button 12 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 13 && button13.Visible)
                        {
                            Console.WriteLine("button 13 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 14 && button14.Visible)
                        {
                            Console.WriteLine("button 14 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 15 && button15.Visible)
                        {
                            Console.WriteLine("button 15 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 16 && button16.Visible)
                        {
                            Console.WriteLine("button 16 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 17 && button17.Visible)
                        {
                            Console.WriteLine("button 17 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 18 && button18.Visible)
                        {
                            Console.WriteLine("button 18 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 19 && button19.Visible)
                        {
                            Console.WriteLine("button 19 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 20 && button20.Visible)
                        {
                            Console.WriteLine("button 20 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 21 && button21.Visible)
                        {
                            Console.WriteLine("button 21 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 22 && button22.Visible)
                        {
                            Console.WriteLine("button 22 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 23 && button23.Visible)
                        {
                            Console.WriteLine("button 23 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 24 && button24.Visible)
                        {
                            Console.WriteLine("button 24 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 25 && button25.Visible)
                        {
                            Console.WriteLine("button 25 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 26 && button26.Visible)
                        {
                            Console.WriteLine("button 26 has Golden Flag");
                            break;
                        }
                        if (GoldenFlag == 27 && button27.Visible)
                        {
                            Console.WriteLine("button 27 has Golden Flag");
                            break;
                        }

                    }

                }
            }
            else if(Round>GoldenFlagRound)
            {
                status.Text = "";
                GoldenFlag = 999;
            }
            Console.WriteLine("GOLDEN FLAG {0}",GoldenFlag);

            //SETUP PROCESS FOR BEGINNING
            if (start)
            {

                setup();
            }


            if (receivedData == "finish")
            {
                opponentScore = 5;
                textBox1.Text = "5";
                status.Text = "YOU LOSE, OPPONENT FOUND THE GOLDEN FLAG";
                disableAll();

            }

            //TAKING INDEX AS A INTEGER TO DETERMINE WHICH BUTTON CLIENT CLICKED
            if (receivedData.Length <= 2)
            {
                //CONVERT INDEX INTO INTEGER
                Console.WriteLine(receivedData);
                int index = Int32.Parse(receivedData);

                //CLICK PROCESS WITH INDEX
                opponentClick(index);

            }
            
           

            
            
   
        }




        // DECLARES BLUE TEAMS FLAGS
            void setup()
        {
            
           //ENABLE ALL BUTTONS
            enableAll();
            //STARTING PROCESS HAS FINISHED CLIENT CLICKED THEN I CAN MAKE THIS FALSE
            start = false;
            for (int j = 0; j < valuesForCity.Length; j++)
            {

                if (j == 0 && valuesForCity[j] == 0)
                {
                    button0.Enabled = false;
                    button0.BackColor = Color.Blue;

                }


                if (j == 1 && valuesForCity[j] == 0)
                {
                    button1.Enabled = false;
                    button1.BackColor = Color.Blue;

                }

                if (j == 2 && valuesForCity[j] == 0)
                {
                    button2.Enabled = false;
                    button2.BackColor = Color.Blue;

                }

                if (j == 3 && valuesForCity[j] == 0)
                {
                    button3.Enabled = false;
                    button3.BackColor = Color.Blue;

                }

                if (j == 4 && valuesForCity[j] == 0)
                {
                    button4.Enabled = false;
                    button4.BackColor = Color.Blue;

                }

                if (j == 5 && valuesForCity[j] == 0)
                {
                    button5.Enabled = false;
                    button5.BackColor = Color.Blue;

                }

                if (j == 6 && valuesForCity[j] == 0)
                {
                    button6.Enabled = false;
                    button6.BackColor = Color.Blue;

                }

                if (j == 7 && valuesForCity[j] == 0)
                {
                    button7.Enabled = false;
                    button7.BackColor = Color.Blue;

                }

                if (j == 8 && valuesForCity[j] == 0)
                {
                    button8.Enabled = false;
                    button8.BackColor = Color.Blue;

                }
                if (j == 9 && valuesForCity[j] == 0)
                {
                    button9.Enabled = false;
                    button9.BackColor = Color.Blue;

                }
                if (j == 10 && valuesForCity[j] == 0)
                {
                    button10.Enabled = false;
                    button10.BackColor = Color.Blue;

                }
                if (j == 11 && valuesForCity[j] == 0)
                {
                    button11.Enabled = false;
                    button11.BackColor = Color.Blue;

                }
                if (j == 12 && valuesForCity[j] == 0)
                {
                    button12.Enabled = false;
                    button12.BackColor = Color.Blue;

                }
                if (j == 13 && valuesForCity[j] == 0)
                {
                    button13.Enabled = false;
                    button13.BackColor = Color.Blue;

                }
                if (j == 14 && valuesForCity[j] == 0)
                {
                    button14.Enabled = false;
                    button14.BackColor = Color.Blue;

                }
                if (j == 15 && valuesForCity[j] == 0)
                {
                    button15.Enabled = false;
                    button15.BackColor = Color.Blue;

                }
                if (j == 16 && valuesForCity[j] == 0)
                {
                    button16.Enabled = false;
                    button16.BackColor = Color.Blue;

                }
                if (j == 17 && valuesForCity[j] == 0)
                {
                    button17.Enabled = false;
                    button17.BackColor = Color.Blue;

                }
                if (j == 18 && valuesForCity[j] == 0)
                {
                    button18.Enabled = false;
                    button18.BackColor = Color.Blue;

                }
                if (j == 19 && valuesForCity[j] == 0)
                {
                    button19.Enabled = false;
                    button19.BackColor = Color.Blue;

                }
                if (j == 20 && valuesForCity[j] == 0)
                {
                    button20.Enabled = false;
                    button20.BackColor = Color.Blue;

                }
                if (j == 21 && valuesForCity[j] == 0)
                {
                    button21.Enabled = false;
                    button21.BackColor = Color.Blue;

                }
                if (j == 22 && valuesForCity[j] == 0)
                {
                    button22.Enabled = false;
                    button22.BackColor = Color.Blue;

                }
                if (j == 23 && valuesForCity[j] == 0)
                {
                    button23.Enabled = false;
                    button23.BackColor = Color.Blue;

                }
                if (j == 24 && valuesForCity[j] == 0)
                {
                    button24.Enabled = false;
                    button24.BackColor = Color.Blue;

                }
                if (j == 25 && valuesForCity[j] == 0)
                {
                    button25.Enabled = false;
                    button25.BackColor = Color.Blue;

                }
                if (j == 26 && valuesForCity[j] == 0)
                {
                    button26.Enabled = false;
                    button26.BackColor = Color.Blue;

                }
                if (j == 27 && valuesForCity[j] == 0)
                {
                    button27.Enabled = false;
                    button27.BackColor = Color.Blue;

                }


            }

        }



        //SENDING CLIKED BUTTON INDEX AS A STRING TO CLIENT
        public void sendNumber(String x)
        {
            //IF I DID NOT WIN GAME WAITCURSOR ENABLE
            if (myScore != 5)
            {
                UseWaitCursor = true;
                disableAll();
            }


            byte[] message1 = Encoding.ASCII.GetBytes(x);
            client.BeginSend(message1, 0, message1.Length, SocketFlags.None,
                        new AsyncCallback(SendData), client);

        }

        private void button0_Click(object sender, EventArgs e)
        {  

            if (valuesForCity[0] == 1)
            {
               //IF I GOT ENEMY FLAG I'M MAKING THAT FLAG MINE
                button0.Enabled = false;
                button0.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    //FINISHING GAME
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                //WRITING SCORE
                myScore1.Text = myScore.ToString();
                //SENDING INDEX TO USE TO CLIENT
                sendNumber("0");

            }
            else if (valuesForCity[0] == 0)
            {

                //IF CITY IS MINE AND ENEMY CLICKED IT, THEN I AM MAKING THAT FLAG RED
                //AND GIVING ENEMY +1 POINTS
                button0.Enabled = false;
                button0.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    //FINISHING GAME
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            else if(valuesForCity[0] == 2)
            {
                if (GoldenFlag == 0)
                {
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";
                    button0.BackColor = Color.Gold;
                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    //IF ENEMY AND I DONT HAVE THAT FLAG THEN REMOVE THAT FLAG
                    sendNumber("0");
                    button0.Visible = false;

                }
                
            }
 
        }

        //EVERY OTHER PROCESS FOR BUTTON CLICK ARE THE SAME****************************************
        

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("1");
            if (valuesForCity[1] == 1)
            {
                
                button1.Enabled = false;
                button1.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("1");
               
            }
             if (valuesForCity[1] == 0)
            {
                button1.Enabled = false;
                button1.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[1] == 2)
            {
                if (GoldenFlag == 1)
                {
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";
                    button1.BackColor = Color.Gold;
                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("1");
                    button1.Visible = false;
                }
            }


            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine(" 2");
            if (valuesForCity[2] == 1)
            {
               
                button2.Enabled = false;
                button2.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("2");
                
            }
             if (valuesForCity[2] == 0)
            {
                button2.Enabled = false;
                button2.BackColor = Color.Red;
                
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[2] == 2)
            {
                if (GoldenFlag == 2)
                {
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";
                    button2.BackColor = Color.Gold;
                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("2");
                    button2.Visible = false;
                }
            }


            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine(" 3");
            if (valuesForCity[3] == 1)
            {
               
                button3.Enabled = false;
                button3.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("3");
                
            }
             if (valuesForCity[3] == 0)
            {
                button3.Enabled = false;
                button3.BackColor = Color.Red;

                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[3] == 2)
            {
                if (GoldenFlag == 3)
                {
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";
                    button3.BackColor = Color.Gold;
                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("3");
                    button3.Visible = false;
                }
            }
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (valuesForCity[4] == 1)
            {
                
                button4.Enabled = false;
                button4.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("4");
                
            }
             if (valuesForCity[4] == 0)
            {
                button4.Enabled = false;
                button4.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[4] == 2)
            {
                if (GoldenFlag == 4)
                {
                    button4.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("4");
                    button4.Visible = false;
                }
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (valuesForCity[5] == 1)
            {
                
                button5.Enabled = false;
                button5.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("5");
                
            }
             if (valuesForCity[5] == 0)
            {
                button5.Enabled = false;
                button5.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[5] == 2)
            {
                if (GoldenFlag == 5)
                {
                    button5.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("5");
                    button5.Visible = false;
                }
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (valuesForCity[6] == 1)
            {
              
                button6.Enabled = false;
                button6.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("6");
                
            }
             if (valuesForCity[6] == 0)
            {
                button6.Enabled = false;
                button6.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[6] == 2)
            {
                if (GoldenFlag == 6)
                {
                    button6.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("6");
                    button6.Visible = false;
                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (valuesForCity[7] == 1)
            {
               
                button7.Enabled = false;
                button7.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("7");
                
            }
             if (valuesForCity[7] == 0)
            {
                button7.Enabled = false;
                button7.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            else if (valuesForCity[7] == 2)
            {
                if (GoldenFlag == 7)
                {
                    button7.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("7");
                    button7.Visible = false;
                }
            }

        }

        

        private void button8_Click(object sender, EventArgs e)
        {

            if (valuesForCity[8] == 1)
            {
                
                button8.Enabled = false;
                button8.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                
                sendNumber("8");
            }
              if (valuesForCity[8] == 0)
            {
                button8.Enabled = false;
                button8.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
             if (valuesForCity[8] == 2)
            {
                if (GoldenFlag == 8)
                {
                    button8.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("8");
                    button8.Visible = false;
                }
            }

        }


       

        

        private void button9_Click(object sender, EventArgs e)
        {
            if (valuesForCity[9] == 1)
            {

                button9.Enabled = false;
                button9.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();

                sendNumber("9");
            }
            if (valuesForCity[9] == 0)
            {
                button9.Enabled = false;
                button9.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[9] == 2)
            {
                if (GoldenFlag == 9)
                {
                    button9.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("9");
                    button9.Visible = false;
                }
            }

        }


        private void button10_Click(object sender, EventArgs e)
        {
            if (valuesForCity[10] == 1)
            {

                button10.Enabled = false;
                button10.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();

                sendNumber("10");
            }
            if (valuesForCity[10] == 0)
            {
                button10.Enabled = false;
                button10.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[10] == 2)
            {
                if (GoldenFlag == 10)
                {
                    button10.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("10");
                    button10.Visible = false;
                }
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (valuesForCity[11] == 1)
            {

                button11.Enabled = false;
                button11.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
               
                sendNumber("11");
            }
            if (valuesForCity[11] == 0)
            {
                button11.Enabled = false;
                button11.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[11] == 2)
            {
                if (GoldenFlag == 11)
                {
                    button11.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("11");
                    button11.Visible = false;
                }
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (valuesForCity[12] == 1)
            {

                button12.Enabled = false;
                button12.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString(); 
                sendNumber("12");
            }
            if (valuesForCity[12] == 0)
            {
                button12.Enabled = false;
                button12.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[12] == 2)
            {
                if (GoldenFlag == 12)
                {
                    button12.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("12");
                    button12.Visible = false;
                }
            }

        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (valuesForCity[13] == 1)
            {

                button13.Enabled = false;
                button13.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("13");
            }
            if (valuesForCity[13] == 0)
            {
                button13.Enabled = false;
                button13.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[13] == 2)
            {
                if (GoldenFlag == 13)
                {
                    button13.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("13");
                    button13.Visible = false;
                }
            }

        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (valuesForCity[14] == 1)
            {

                button14.Enabled = false;
                button14.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("14");
            }
            if (valuesForCity[14] == 0)
            {
                button14.Enabled = false;
                button14.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[14] == 2)
            {
                if (GoldenFlag == 14)
                {
                    button14.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("14");
                    button14.Visible = false;
                }
            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (valuesForCity[15] == 1)
            {

                button15.Enabled = false;
                button15.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("15");
            }
            if (valuesForCity[15] == 0)
            {
                button15.Enabled = false;
                button15.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[15] == 2)
            {
                if (GoldenFlag == 15)
                {
                    button15.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("15");
                    button15.Visible = false;
                }
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (valuesForCity[16] == 1)
            {

                button16.Enabled = false;
                button16.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("16");
            }
            if (valuesForCity[16] == 0)
            {
                button16.Enabled = false;
                button16.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[16] == 2)
            {
                if (GoldenFlag == 16)
                {
                    button16.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("16");
                    button16.Visible = false;
                }
            }

        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (valuesForCity[17] == 1)
            {

                button17.Enabled = false;
                button17.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("17");
            }
            if (valuesForCity[17] == 0)
            {
                button17.Enabled = false;
                button17.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[17] == 2)
            {
                if (GoldenFlag == 17)
                {
                    button17.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("17");
                    button17.Visible = false;
                }
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (valuesForCity[18] == 1)
            {

                button18.Enabled = false;
                button18.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("18");
            }
            if (valuesForCity[18] == 0)
            {
                button18.Enabled = false;
                button18.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[18] == 2)
            {
                if (GoldenFlag == 18)
                {
                    button18.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("18");
                    button18.Visible = false;
                }
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (valuesForCity[19] == 1)
            {

                button19.Enabled = false;
                button19.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("19");
            }
            if (valuesForCity[19] == 0)
            {
                button19.Enabled = false;
                button19.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[19] == 2)
            {
                if (GoldenFlag == 19)
                {
                    button19.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("19");
                    button19.Visible = false;
                }
            }

        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (valuesForCity[20] == 1)
            {
                button20.Enabled = false;
                button20.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();
                }
                myScore1.Text = myScore.ToString();
                sendNumber("20");
            }
            if (valuesForCity[20] == 0)
            {
                button20.Enabled = false;
                button20.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();
                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[20] == 2)
            {
                if (GoldenFlag == 20)
                {
                    button20.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";
                    disableAll();
                    sendNumber("finish");
                }
                else
                {
                    sendNumber("20");
                    button20.Visible = false;
                }
            }

        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (valuesForCity[21] == 1)
            {

                button21.Enabled = false;
                button21.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("21");
            }
            if (valuesForCity[21] == 0)
            {
                button21.Enabled = false;
                button21.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[21] == 2)
            {
                if (GoldenFlag == 21)
                {
                    button21.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("21");
                    button21.Visible = false;
                }
            }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (valuesForCity[22] == 1)
            {

                button22.Enabled = false;
                button22.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("22");
            }
            if (valuesForCity[22] == 0)
            {
                button22.Enabled = false;
                button22.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[22] == 2)
            {
                if (GoldenFlag == 22)
                {
                    button22.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("22");
                    button22.Visible = false;
                }
            }

        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (valuesForCity[23] == 1)
            {

                button23.Enabled = false;
                button23.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("23");
            }
            if (valuesForCity[23] == 0)
            {
                button23.Enabled = false;
                button23.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[23] == 2)
            {
                if (GoldenFlag == 23)
                {
                    button23.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("23");
                    button23.Visible = false;
                }
            }

        }
        private void button24_Click(object sender, EventArgs e)
        {
            if (valuesForCity[24] == 1)
            {

                button24.Enabled = false;
                button24.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("24");
            }
            if (valuesForCity[24] == 0)
            {
                button24.Enabled = false;
                button24.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[24] == 2)
            {
                if (GoldenFlag == 24)
                {
                    button24.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("24");
                    button24.Visible = false;
                }
            }

        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (valuesForCity[25] == 1)
            {

                button25.Enabled = false;
                button25.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("25");
            }
            if (valuesForCity[25] == 0)
            {
                button25.Enabled = false;
                button25.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[25] == 2)
            {
                if (GoldenFlag == 25)
                {
                    button25.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("25");
                    button25.Visible = false;
                }
            }

        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (valuesForCity[26] == 1)
            {

                button26.Enabled = false;
                button26.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("26");
            }
            if (valuesForCity[26] == 0)
            {
                button26.Enabled = false;
                button26.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[26] == 2)
            {
                if (GoldenFlag == 26)
                {
                    button26.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("26");
                    button26.Visible = false;
                }
            }

        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (valuesForCity[27] == 1)
            {

                button27.Enabled = false;
                button27.BackColor = Color.Blue;
                myScore++;
                if (myScore == 5)
                {
                    status.Text = "YOU WIN !";
                    disableAll();

                }
                myScore1.Text = myScore.ToString();
                sendNumber("27");
            }
            if (valuesForCity[27] == 0)
            {
                button27.Enabled = false;
                button27.BackColor = Color.Red;
                opponentScore++;
                if (opponentScore == 5)
                {
                    status.Text = "YOU LOSE !";
                    disableAll();

                }
                textBox1.Text = opponentScore.ToString();
            }
            if (valuesForCity[27] == 2)
            {
                if (GoldenFlag == 27)
                {
                    button27.BackColor = Color.Gold;
                    myScore = 5;
                    myScore1.Text = "5";
                    status.Text = "YOU WIN, YOU FOUND THE GOLDEN FLAG!";

                    disableAll();
                    sendNumber("finish");

                }
                else
                {
                    sendNumber("27");
                    button27.Visible = false;
                }
            }

        }


        //WHEN OPPONENT CLICKED ONE FLAG I AM TAKING INDEX AND MAKING CLICK OPERATION ON MY FLAGS
        public void opponentClick(int index)
        {
           
            //IF I HAVE THAT CITY I HAVE TO MAKE ENABLE FIRST
            if (index == 0 && button0.Visible == true && myCity.Contains(index))
            {
                button0.Enabled = true;
                button0.PerformClick();

            }
            //IF I DO NOT HAVE THAT CITY THEN I HAVE TO REMOVE THAT BUTTON
            else if (index == 0 && button0.Visible == true && !myCity.Contains(index))
            {
                button0.Visible = false;

            }
            //ALL OTHER PROCESS  IN THIS SCOPE ARE THE SAME FOR BUTTONS**************************
            if (index == 1 && button1.Visible == true && myCity.Contains(index))
            {
                button1.Enabled = true;
                button1.PerformClick();



            }
            else if (index == 1 && button1.Visible == true && !myCity.Contains(index))
            {
                button1.Visible = false;

            }

            if (index == 2 && button2.Visible == true && myCity.Contains(index))
            {
                button2.Enabled = true;
                button2.PerformClick();


            }
            else if (index == 2 && button2.Visible == true && !myCity.Contains(index))
            {
                button2.Visible = false;

            }
            if (index == 3 && button3.Visible == true && myCity.Contains(index))
            {
                button3.Enabled = true;
                button3.PerformClick();


            }
            else if (index == 3 && button3.Visible == true && !myCity.Contains(index))
            {
                button3.Visible = false;

            }
            if (index == 4 && button4.Visible == true && myCity.Contains(index))
            {
                button4.Enabled = true;
                button4.PerformClick();


            }
            else if (index == 4 && button4.Visible == true && !myCity.Contains(index))
            {
                button4.Visible = false;

            }
            if (index == 5 && button5.Visible == true && myCity.Contains(index))
            {
                button5.Enabled = true;
                button5.PerformClick();


            }
            else if (index == 5 && button5.Visible == true && !myCity.Contains(index))
            {
                button5.Visible = false;

            }
            if (index == 6 && button6.Visible == true && myCity.Contains(index))
            {
                button6.Enabled = true;
                button6.PerformClick();


            }
            else if (index == 6 && button6.Visible == true && !myCity.Contains(index))
            {
                button6.Visible = false;

            }
            if (index == 7 && button7.Visible == true && myCity.Contains(index))
            {
                button8.Enabled = true;
                button7.PerformClick();


            }
            else if (index == 7 && button7.Visible == true && !myCity.Contains(index))
            {
                button7.Visible = false;

            }
            if (index == 8 && button8.Visible == true && myCity.Contains(index))
            {
                button8.Enabled = true;
                button8.PerformClick();


            }
            else if (index == 8 && button8.Visible == true && !myCity.Contains(index))
            {
                button8.Visible = false;

            }
            if (index == 9 && button9.Visible == true && myCity.Contains(index))
            {
                button9.Enabled = true;
                button9.PerformClick();


            }
            else if (index == 9 && button9.Visible == true && !myCity.Contains(index))
            {
                button9.Visible = false;

            }

            if (index == 10 && button10.Visible == true && myCity.Contains(index))
            {
                button10.Enabled = true;
                button10.PerformClick();


            }
            else if (index == 10 && button10.Visible == true && !myCity.Contains(index))
            {
                button10.Visible = false;

            }

            if (index == 11 && button11.Visible == true && myCity.Contains(index))
            {
                button11.Enabled = true;
                button11.PerformClick();


            }
            else if (index == 11 && button11.Visible == true && !myCity.Contains(index))
            {
                button11.Visible = false;

            }

            if (index == 12 && button12.Visible == true && myCity.Contains(index))
            {
                button12.Enabled = true;
                button12.PerformClick();


            }
            else if (index == 12 && button12.Visible == true && !myCity.Contains(index))
            {
                button12.Visible = false;

            }

            if (index == 13 && button13.Visible == true && myCity.Contains(index))
            {
                button13.Enabled = true;
                button13.PerformClick();


            }
            else if (index == 13 && button13.Visible == true && !myCity.Contains(index))
            {
                button13.Visible = false;

            }

            if (index == 14 && button14.Visible == true && myCity.Contains(index))
            {
                button14.Enabled = true;
                button14.PerformClick();


            }
            else if (index == 14 && button14.Visible == true && !myCity.Contains(index))
            {
                button14.Visible = false;

            }

            if (index == 15 && button15.Visible == true && myCity.Contains(index))
            {
                button15.Enabled = true;
                button15.PerformClick();


            }
            else if (index == 15 && button15.Visible == true && !myCity.Contains(index))
            {
                button15.Visible = false;

            }

            if (index == 16 && button16.Visible == true && myCity.Contains(index))
            {
                button16.Enabled = true;
                button16.PerformClick();


            }
            else if (index == 16 && button16.Visible == true && !myCity.Contains(index))
            {
                button16.Visible = false;

            }

            if (index == 17 && button17.Visible == true && myCity.Contains(index))
            {
                button17.Enabled = true;
                button17.PerformClick();


            }
            else if (index == 17 && button17.Visible == true && !myCity.Contains(index))
            {
                button17.Visible = false;

            }

            if (index == 18 && button18.Visible == true && myCity.Contains(index))
            {
                button18.Enabled = true;
                button18.PerformClick();


            }
            else if (index == 18 && button18.Visible == true && !myCity.Contains(index))
            {
                button18.Visible = false;

            }

            if (index == 19 && button19.Visible == true && myCity.Contains(index))
            {
                button19.Enabled = true;
                button19.PerformClick();


            }
            else if (index == 19 && button19.Visible == true && !myCity.Contains(index))
            {
                button19.Visible = false;

            }

            if (index == 20 && button20.Visible == true && myCity.Contains(index))
            {
                button20.Enabled = true;
                button20.PerformClick();


            }
            else if (index == 20 && button20.Visible == true && !myCity.Contains(index))
            {
                button20.Visible = false;

            }

            if (index == 21 && button21.Visible == true && myCity.Contains(index))
            {
                button21.Enabled = true;
                button21.PerformClick();


            }
            else if (index == 21 && button21.Visible == true && !myCity.Contains(index))
            {
                button21.Visible = false;

            }

            if (index == 22 && button22.Visible == true && myCity.Contains(index))
            {
                button22.Enabled = true;
                button22.PerformClick();


            }
            else if (index == 22 && button22.Visible == true && !myCity.Contains(index))
            {
                button22.Visible = false;

            }

            if (index == 23 && button23.Visible == true && myCity.Contains(index))
            {
                button23.Enabled = true;
                button23.PerformClick();


            }
            else if (index == 23 && button23.Visible == true && !myCity.Contains(index))
            {
                button23.Visible = false;

            }
            if (index == 24 && button24.Visible == true && myCity.Contains(index))
            {
                button24.Enabled = true;
                button24.PerformClick();


            }
            else if (index == 24 && button24.Visible == true && !myCity.Contains(index))
            {
                button24.Visible = false;

            }
            if (index == 25 && button25.Visible == true && myCity.Contains(index))
            {
                button25.Enabled = true;
                button25.PerformClick();


            }
            else if (index == 25 && button25.Visible == true && !myCity.Contains(index))
            {
                button25.Visible = false;

            }
            if (index == 26 && button26.Visible == true && myCity.Contains(index))
            {
                button26.Enabled = true;
                button26.PerformClick();


            }
            else if (index == 26 && button26.Visible == true && !myCity.Contains(index))
            {
                button26.Visible = false;

            }
            if (index == 27 && button27.Visible == true && myCity.Contains(index))
            {
                button27.Enabled = true;
                button27.PerformClick();


            }
            else if (index == 27 && button27.Visible == true && !myCity.Contains(index))
            {
                button27.Visible = false;

            }





        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

       

        void disableAll()
        {
            button0.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;
            button18.Enabled = false;
            button19.Enabled = false;
            button20.Enabled = false;
            button21.Enabled = false;
            button22.Enabled = false;
            button23.Enabled = false;
            button24.Enabled = false;
            button25.Enabled = false;
            button26.Enabled = false;
            button27.Enabled = false;
        }
        void enableAll()
        {
            if(button0.Visible && (button0.BackColor!=Color.Red && button0.BackColor != Color.Blue)){
                button0.Enabled = true;
            }
            if (button1.Visible && (button1.BackColor != Color.Red && button1.BackColor != Color.Blue))
            {
                button1.Enabled = true;
            }
            if (button2.Visible && (button2.BackColor != Color.Red && button2.BackColor != Color.Blue))
            {
                button2.Enabled = true;
            }
            if (button3.Visible && (button3.BackColor != Color.Red && button3.BackColor != Color.Blue))
            {
                button3.Enabled = true;
            }
            if (button4.Visible && (button4.BackColor != Color.Red && button4.BackColor != Color.Blue))
            {
                button4.Enabled = true;
            }
            if (button5.Visible && (button5.BackColor != Color.Red && button5.BackColor != Color.Blue))
            {
                button5.Enabled = true;
            }
            if (button6.Visible && (button6.BackColor != Color.Red && button6.BackColor != Color.Blue))
            {
                button6.Enabled = true;
            }
            if (button7.Visible && (button7.BackColor != Color.Red && button7.BackColor != Color.Blue))
            {
                button7.Enabled = true;
            }
            if (button8.Visible && (button8.BackColor != Color.Red && button8.BackColor != Color.Blue))
            {
                button8.Enabled = true;
            }
            if (button9.Visible && (button9.BackColor != Color.Red && button9.BackColor != Color.Blue))
            {
                button9.Enabled = true;
            }
            if (button10.Visible && (button10.BackColor != Color.Red && button10.BackColor != Color.Blue))
            {
                button10.Enabled = true;
            }
            if (button11.Visible && (button11.BackColor != Color.Red && button11.BackColor != Color.Blue))
            {
                button11.Enabled = true;
            }
            if (button12.Visible && (button12.BackColor != Color.Red && button12.BackColor != Color.Blue))
            {
                button12.Enabled = true;
            }
            if (button13.Visible && (button13.BackColor != Color.Red && button13.BackColor != Color.Blue))
            {
                button13.Enabled = true;
            }
            if (button14.Visible && (button14.BackColor != Color.Red && button14.BackColor != Color.Blue))
            {
                button14.Enabled = true;
            }
            if (button15.Visible && (button15.BackColor != Color.Red && button15.BackColor != Color.Blue))
            {
                button15.Enabled = true;
            }
            if (button16.Visible && (button16.BackColor != Color.Red && button16.BackColor != Color.Blue))
            {
                button16.Enabled = true;
            }
            if (button17.Visible && (button17.BackColor != Color.Red && button17.BackColor != Color.Blue))
            {
                button17.Enabled = true;
            }
            if (button18.Visible && (button18.BackColor != Color.Red && button18.BackColor != Color.Blue))
            {
                button18.Enabled = true;
            }
            if (button19.Visible && (button19.BackColor != Color.Red && button19.BackColor != Color.Blue))
            {
                button19.Enabled = true;
            }
            if (button20.Visible && (button20.BackColor != Color.Red && button20.BackColor != Color.Blue))
            {
                button20.Enabled = true;
            }
            if (button21.Visible && (button21.BackColor != Color.Red && button21.BackColor != Color.Blue))
            {
                button21.Enabled = true;
            }
            if (button22.Visible && (button22.BackColor != Color.Red && button22.BackColor != Color.Blue))
            {
                button22.Enabled = true;
            }
            if (button23.Visible && (button23.BackColor != Color.Red && button23.BackColor != Color.Blue))
            {
                button23.Enabled = true;
            }
            if (button24.Visible && (button24.BackColor != Color.Red && button24.BackColor != Color.Blue))
            {
                button24.Enabled = true;
            }
            if (button25.Visible && (button25.BackColor != Color.Red && button25.BackColor != Color.Blue))
            {
                button25.Enabled = true;
            }
            if (button26.Visible && (button26.BackColor != Color.Red && button26.BackColor != Color.Blue))
            {
                button26.Enabled = true;
            }
            if (button27.Visible && (button27.BackColor != Color.Red && button27.BackColor != Color.Blue))
            {
                button27.Enabled = true;
            }
            


           
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void myScore_TextChanged(object sender, EventArgs e)
        {

        }

        private void status_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
