using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Zeki_Arslan_Socket
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public const int PortTakip = 38000; // Bu uygulamanın dinleyeceği port.
        public Thread thread;

        delegate void clientdg(string datax);
        public int i = 1;
        TcpListener tcplisten;

        public void Client_Listen()
        {
            tcplisten = new TcpListener(PortTakip);//Tcplistener tanımla dinlenecek port numarasını gir
            try
            {
                while (true)
                {
                    tcplisten.Start(); //dinlemeye başla
                    Socket socket1 = tcplisten.AcceptSocket();//socket nesnesini porta bağla
                    Byte[] okunan = new Byte[1024];
                    int boyut = socket1.Receive(okunan, okunan.Length, 0);//socketdeki veriyi bayt dizisine aktar
                    string dataokunan = System.Text.Encoding.Default.GetString(okunan);//bayt dizisindeki değeri stringe çevir, dil ayarını yap
                    String gidenveri = "Konuşmacı: Mesajınız Alınmıştır";// Karşı tarafa bir mesaj göndersin
                    Byte[] gidenbyte = System.Text.Encoding.Default.GetBytes(gidenveri.ToCharArray());//Gönderilecek mesaj bayt dizisine alınsın
                    socket1.Send(gidenbyte, gidenbyte.Length, 0);//socket veriyi göndersin
                    i++;// kaç değer geldi saysın
                    Clientproxy(dataokunan);//delege yordamlarında listboa eklensin
                    Clientproxy2(socket1.RemoteEndPoint.ToString());
                }
            }
            catch (SocketException ec)
            {
                MessageBox.Show("Veri Alımında Hata");
            }
        }


        private void Clientproxy(string veri)
        {
            if (this.listBox1.InvokeRequired)
            {
                clientdg delx = new clientdg(Clientproxy);
                this.listBox1.Invoke(delx, new object[] { veri });
            }
            else
            {
                listBox1.Items.Add(veri);
                listBox1.Update();
            }
        }

        private void Clientproxy2(string dtx)
        {
            if (this.listBox2.InvokeRequired)
            {
                clientdg dlg = new clientdg(Clientproxy2);
                this.listBox2.Invoke(dlg, new object[] { dtx });
            }
            else
            {
                listBox2.Items.Add(dtx);
                listBox2.Update();
            }
        }
        private int kx = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Uzak (Konuşmacı) Bilgisayar";
            label4.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient tcpcln = new TcpClient("192.168.1.105", 28015);//Tcpclient nesnesi yaratılarak veri gönderilecek PC nin Ip ve port numarası atanıyor
                NetworkStream netws = tcpcln.GetStream();//Networkstream nesnesi oluşturuluyor
                if (netws.CanWrite)//yazma modunda ise
                {
                    Byte[] giden = Encoding.Default.GetBytes("Konuşmacı >> " + textBox1.Text);
                    netws.Write(giden, 0, giden.Length);
                    Clientproxy("Konuşmacı >> " + textBox1.Text);
                    Clientproxy2("Siz");
                }
                else
                {
                    tcpcln.Close();
                    return;
                }
                if (netws.CanRead)//okuma modunda ise karşı makinenin gönderdiği alındı mesajı labela yazılsın
                {
                    byte[] oku = new byte[tcpcln.ReceiveBufferSize];//alındı mesajının büyüklüğüne göre dizi yarat
                    netws.Read(oku, 0, (int)tcpcln.ReceiveBufferSize);//Networkstream nesnesindeki veriyi bayt dizisine aktar
                    string gelen = Encoding.Default.GetString(oku);//bayt dizisine aktarılan veriyi dil ayarlarını yaparak stringe aktar
                    tcpcln.Close();//tcpclient ile açılan portu kapat
                    label4.Text = kx.ToString() + " " + gelen;
                }
                else
                {
                    tcpcln.Close();
                    return;
                }
                kx++;
            }
            catch
            {
                MessageBox.Show("Veri Gönderme Hatası");
            }
            textBox1.Clear();
        }
        ThreadStart listenx;

        private void button2_Click(object sender, EventArgs e)
        {
            thread = new Thread(Client_Listen);
            thread.Priority = ThreadPriority.Normal;
            thread.IsBackground = true;
            thread.Start();
            button2.Text = "Port Dinleniyor.";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
        }
    }
}
