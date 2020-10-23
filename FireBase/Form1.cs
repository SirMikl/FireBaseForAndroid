using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace FireBase
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        //добавлен комментарий
        IFirebaseConfig config = new FirebaseConfig 
        { 
            AuthSecret = "lmVqXev9VDCZgrABb67BIKr8ECoKcguhXevBQEBa",
            BasePath = "https://android-fcm-de7cc.firebaseio.com/"
        };
        IFirebaseClient client;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);
            if (client != null) 
            {
                MessageBox.Show("Connected!");
            }
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("adress");
            dt.Columns.Add("age");
            dt.Columns.Add("Image", typeof(Image));
            dataGridView1.DataSource = dt;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            FirebaseResponse resp = await client.GetTaskAsync("Counter/node");
            Counter_class get = resp.ResultAs<Counter_class>();
            MemoryStream ms = new MemoryStream();
            imageBox.Image.Save(ms, ImageFormat.Jpeg);
            byte[] a = ms.GetBuffer();
            string output = Convert.ToBase64String(a);
            var data = new Data
            {
                Id = (Convert.ToInt32(get.cnt)+1).ToString(),
                Name = textBox2.Text,
                Adress = textBox3.Text,
                Age = textBox4.Text,
                Img=output
            };
            SetResponse response = await client.SetTaskAsync("Information/" + data.Id, data);
            Data result = response.ResultAs<Data>();
            MessageBox.Show("Data inserted" + result.Id);
            var obj = new Counter_class 
            { 
                cnt = data.Id 
            };
            SetResponse response1 = await client.SetTaskAsync("Counter/node", obj);
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetTaskAsync("Information/"+textBox1.Text);
            Data obj = response.ResultAs<Data>();
            textBox1.Text = obj.Id;
            textBox2.Text = obj.Name;
            textBox3.Text = obj.Adress;
            textBox4.Text = obj.Age;
            MessageBox.Show("Data retieved succsesfully");
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                Id = textBox1.Text,
                Name = textBox2.Text,
                Adress = textBox3.Text,
                Age = textBox4.Text
            };
            FirebaseResponse response = await client.UpdateTaskAsync("Information/" + textBox1.Text,data);
            Data result = response.ResultAs<Data>();
            MessageBox.Show("Data updated at ID: " + result.Id);
        }
        private async void button4_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteTaskAsync("Information/" + textBox1.Text);
            MessageBox.Show("Deleted record at ID: " + textBox1.Text);
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.DeleteTaskAsync("Information");
            MessageBox.Show("All elements deleted");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            export();
        }
        private async void export()
        {
            dt.Rows.Clear();
            int i = 0;
            FirebaseResponse resp1 = await client.GetTaskAsync("Counter/node");
            Counter_class obj1 = resp1.ResultAs<Counter_class>();
            int cnt = Convert.ToInt32(obj1.cnt);
            while (true)
            {
                if (i==cnt)
                {
                    break;
                }
                i++;
                try
                {
                    FirebaseResponse resp2 = await client.GetTaskAsync("Information/" + i);
                    Data obj2 = resp2.ResultAs<Data>();
                    DataRow row = dt.NewRow();
                    row["id"] = obj2.Id;
                    row["name"] = obj2.Name;
                    row["adress"] = obj2.Adress;
                    row["age"] = obj2.Age;
                    byte[] b = Convert.FromBase64String(obj2.Img);
                    MemoryStream ms = new MemoryStream();
                    ms.Write(b, 0, Convert.ToInt32(b.Length));
                    Bitmap bm = new Bitmap(ms, false);
                    row["Image"] = bm;
                    dt.Rows.Add(row);
                }
                catch (Exception)
                {

                }
            }
            MessageBox.Show("Done!");
        }
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select image";
            ofd.Filter = "Image Files(*.jpg) | *.jpg";
            if (ofd.ShowDialog() == DialogResult.OK) 
            {
                Image img = new Bitmap(ofd.FileName);
                imageBox.Image = img.GetThumbnailImage(350, 200, null, new IntPtr());
            }
        }
        private async void button8_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            imageBox.Image.Save(ms, ImageFormat.Jpeg);
            byte[] a = ms.GetBuffer();
            string output = Convert.ToBase64String(a);
            var data = new Image_model
            {
                Img = output
            };
            SetResponse response = await client.SetTaskAsync("Image/", data);
            Image_model result = response.ResultAs<Image_model>();
            imageBox.Image = null;
            MessageBox.Show("Image inserted");
        }
        private async void button9_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetTaskAsync("Image/");
            Image_model image = response.ResultAs<Image_model>();
            byte[] b = Convert.FromBase64String(image.Img);
            MemoryStream ms = new MemoryStream();
            ms.Write(b, 0, Convert.ToInt32(b.Length));
            Bitmap bm = new Bitmap(ms, false);
            ms.Dispose();
            imageBox.Image = bm;
        }
    }
}
