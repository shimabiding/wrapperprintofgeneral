using System;
using System.Windows.Forms;
using System.Drawing;

namespace wgp
{
public partial class Form1 : Form{
    Panel panel1;
    TextBox textBox1;
    Button button1;
    GroupBox groupBox1;
    RadioButton radioButton1;
    RadioButton radioButton2;
    public Form1(){
        //InitializeComponent();
        CALControls();
    }

    void CALControls(){
        this.Text = "GeneralPrintWrapper";
        this.ClientSize = new System.Drawing.Size(400, 300);

        this.Controls.Add(panel1 = new Panel{
            Location = new Point(10, 10),
            BackColor = Color.Azure,
            Size = new Size(300, 200),
        });

        panel1.Controls.Add(groupBox1 = new GroupBox{
            Location = new Point(10, 60),
            Size = new Size(100, 100),
            Text = "rbBox",
        });

        groupBox1.Controls.Add(radioButton1 = new RadioButton{
            Location = new Point(0, 18),
            Size = new Size(100, 20),
            Text = "rb1",
        });

        groupBox1.Controls.Add(radioButton2 = new RadioButton{
            Location = new Point(0, 40),
            Size = new Size(100, 20),
            Text = "rb2",
        });

        panel1.Controls.Add(textBox1 = new TextBox{
            Name = "textBox1",
            Location = new Point(5, 0),
            Size = new Size(200, 22),
        });

        panel1.Controls.Add(button1 = new Button{
            Location = new Point(5, 23),
            Size = new Size(200, 22),
            Text = "botann",
            BackColor = Color.Crimson,
        });

        button1.Click += Button1_Click;

        panel1.BringToFront();
    }

    private void Button1_Click(object sender, EventArgs e){
        MessageBox.Show(textBox1.Text);
    }
}


}
