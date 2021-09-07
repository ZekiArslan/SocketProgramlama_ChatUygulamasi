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

        public const int portno = 28015; //Dinlenecek port numarası
        public Thread kanal;

        delegate void STD(string datax); //delege tanımlaması
        public int i = 1;
        Socket socket1;

        public void SWT_Listen()
        {
            TcpListener tcplisten = new TcpListener(portno); //Tcp dinleme nesnesi tanımlama
            try
            {
                while (true)
                {
                    tcplisten.Start();//dinlemeye başla
                    socket1 = tcplisten.AcceptSocket();//dinlenen portu socket nesnesine bağla
                    Byte[] okunan = new Byte[1024];
                    int boyut = socket1.Receive(okunan, okunan.Length, 0);//socket deki değeri al ve bayt dizisine geçir
                    string dataokunan = System.Text.Encoding.Default.GetString(okunan);//alınan değeri stringe geçir, dil ayarını yap
                    String gidenveri = "Sunucu: Mesajınız Alınmıştır"; //karşı tarafa bir alındı mesajı hazırla
                    Byte[] gidenbyte = System.Text.Encoding.Default.GetBytes(gidenveri.ToCharArray());//alındı mesajını bayt dizisine aktar
                    socket1.Send(gidenbyte, gidenbyte.Length, 0);//socket nesnesi ile alındı mesajını yolla
                    i++;//kaç mesaj geldiğini say
                    Kanalproxsy(dataokunan);//delege kontrolüne gönder ki listboxa geçirsin
                    Kanalproxsy2(socket1.RemoteEndPoint.ToString());//2. delege yordamı da listbox2 ye IP numarasını yazsın

                }
            }
            catch (SocketException ec)
            {
                MessageBox.Show("TCP Dinleme Hatası");//Hata olursa bildir
            }
        }

        private void Kanalproxsy(string veri)
        {
            if (this.listBox1.InvokeRequired)
            {
                STD delx = new STD(Kanalproxsy);
                this.listBox1.Invoke(delx, new object[] { veri });
            }
            else
            {
                listBox1.Items.Add(veri);
                listBox1.Update();
            }
        }
        private void Kanalproxsy2(string dtx)
        {
            if (this.listBox2.InvokeRequired)
            {
                STD dlg = new STD(Kanalproxsy2);
                this.listBox2.Invoke(dlg, new object[] { dtx });
            }
            else
            {
                listBox2.Items.Add(dtx);
                listBox2.Update();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Yakın (Server) Bilgisayar";
            label4.Text = "";
        }
        private int kx = 1;
        TcpClient tcpcln;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                tcpcln = new TcpClient("192.168.1.105", 38000); //veri gönderebilmek ve okuyabilmek için bir sınıf yarat. Alıcının IP numarasını ve dinlediği port numarasını yaz

                NetworkStream netws = tcpcln.GetStream(); //networkstream nesnesi yarat
                if (netws.CanWrite)//yazım modunda ise
                {
                    Byte[] giden = Encoding.Default.GetBytes("Sunucu >> " + textBox1.Text);//text boxtaki yazıyı bayt dizisine çevir
                    netws.Write(giden, 0, giden.Length);//gönder
                    Kanalproxsy("Sunucu >> " + textBox1.Text);//karşıya giden mesaj da listbox1 de yer alsın
                    Kanalproxsy2("Siz");
                }
                else
                {
                    tcpcln.Close();
                    return;
                }
                if (netws.CanRead)//Okuma modunda ise karşı tarafın alındı mesajını labele yaz
                {
                    byte[] oku = new byte[tcpcln.ReceiveBufferSize];
                    netws.Read(oku, 0, (int)tcpcln.ReceiveBufferSize);
                    string gelen = Encoding.Default.GetString(oku);
                    tcpcln.Close();
                    label4.Text = kx + " " + gelen;
                }
                else
                {
                    tcpcln.Close();
                    return;
                }
                kx++;//Karşı tarafa iletilen mesaj sayısı
            }
            catch
            {
                MessageBox.Show("Veri Gönderim Hatası");

            }
            textBox1.Clear();
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            kanal = new Thread(SWT_Listen); //Bir thread nesnesi yarat. Nesnenin değişkeni tcp yi dinleme prosedürü olsun
            kanal.Priority = ThreadPriority.Normal;
            kanal.IsBackground = true;//Kitlenme Önlemi
            kanal.Start();//başlat
            button2.Text = "Port Dinleniyor.";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();//listboxu temizle
            listBox2.Items.Clear();
        }
    }
}
