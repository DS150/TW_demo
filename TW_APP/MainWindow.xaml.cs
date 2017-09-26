using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

/// <summary>
/// to start: sender, accountSid and authToken need to be set. In realitiy they should be hidden, but this is a demo app
/// in data.txt the first line must be the phone number with leading +
/// </summary>

namespace TW_APP
{
    public partial class MainWindow : Window
    {
        int code_sent;
        string[] file_lines;
        string lines;
        string line;
        int j = 0;
        string phone_number;


        public MainWindow()
        {

            InitializeComponent();
            
            using (StreamReader initial_data = new StreamReader("data.txt"))
            {
                while ((line = initial_data.ReadLine()) != null) //in reality this should be taken from a DB instead. To simplify things it's taken from a file and later saved to it
                {
                    if (j == 0) phoneBox.Text = line;
                    if (j == 1) bankBox.Text = line;
                    if (j == 2) firstnameBox.Text = line;
                    if (j == 3) lastnameBox.Text = line;
                    if (j == 4) emailBox.Text = line;
                    j++;
                }
            }
            lines = phoneBox.Text + "\r\n" + bankBox.Text + "\r\n" + firstnameBox.Text + "\r\n" + lastnameBox.Text + "\r\n" + emailBox.Text;
            phone_number = phoneBox.Text;
            button_code.IsEnabled = !System.String.IsNullOrWhiteSpace(code1.Text);

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
          
        }

        private void tx5_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            file_lines = new string[5];
            string line;
            int counter = 0;
            string body = "Hello, this is the request to change the following data of your Domain account:\n";
            int change = 0; // if it's not 0, than a change was made
            StreamReader sr;
            try
            {   // Open the text file using a stream reader.
                using (sr = new StreamReader("data.txt"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        file_lines[counter] = line;
                        counter++;
                        //logBox.AppendText(line + "\n");
                    }
                }
                sr.Close();
                if (phoneBox.Text != file_lines[0])  // in reality, if changed by user - it should be prevented that these changes are actually successful - create interal alert
                {
                        logBox.AppendText("Phone was changed!!!! ALARM ALARM \n");
                        body += "Phone \n"; // here this code exists since it's only a demo app
                        change = 1;
                    }
                if (bankBox.Text != file_lines[1])
                    {
                        logBox.AppendText("Bank account was changed \n");
                        body += "Bank account\n";
                        change = 1;
                    }
                if (firstnameBox.Text != file_lines[2])
                    {
                        logBox.AppendText("First name was changed \n");
                        body += "First name\n";
                        change = 1;
                    }
                if (lastnameBox.Text != file_lines[3])
                    {
                        logBox.AppendText("Last name was changed \n");
                        body += "Last name\n";
                        change = 1;
                    }
                if (emailBox.Text != file_lines[4])
                    {
                        logBox.AppendText("E-mail was changed \n");
                        body += "e-mail\n";
                        change = 1;
                    }
            }
            catch (Exception ex)
            {
                logBox.AppendText("The file could not be read:");
                logBox.AppendText(ex.Message);
            }
            Random r1 = new Random();
            code_sent=r1.Next(1000, 10000);

            if (change == 1)
            {
                body += "Please insert the following code in the web page to change the data:" ;
                logBox.AppendText("NEW REQUEST: Sent request for message with text: " + body + "####"); //just to log it without code being visible
                body += code_sent;
                logBox.AppendText(SendSMS_Security(body,phone_number, SidBox.Text, TokenBox.Text, senderBox.Text));
                logBox.ScrollToEnd();
            }
            else logBox.AppendText("No change done, no SMS sent.\n");
            
        }

        private static string SendSMS_Security(string message_body, string phone_SMS, string sid, string token, string sender)
        {
            // Initialize the Twilio client
            TwilioClient.Init(sid, token);

            // make an associative array of people we know, indexed by phone number
            var people = new Dictionary<string, string>() {
                    {phone_SMS, "Me"}
                };

            // Iterate over all our friends
            try
            {
                foreach (var person in people)
                {
                    // Send a new outgoing SMS by POSTing to the Messages resource
                    MessageResource.Create(
                         from: new PhoneNumber(sender), // From number, must be an SMS-enabled Twilio number
                         to: new PhoneNumber(person.Key), // To number, if using Sandbox see note above
                                                          // Message content
                         body: $"{message_body}");


                    return ($"\nSent message to {person.Key}\n ................................... \n ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No message sent. Error:"+ ex.Message);
            }
            return ("\n ................................... \n");
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(code1.Text, "[^0-9]")) //allow only numbers in the textBox for the received code
            {
                //MessageBox.Show("Please enter only numbers.");
                code1.Text = code1.Text.Remove(code1.Text.Length - 1);
                code1.SelectionStart = code1.Text.Length;
            }
            if (code1.Text.Length >4) // allow only 4 digits
            {
                code1.Text = code1.Text.Remove(code1.Text.Length - 1);
                code1.SelectionStart = code1.Text.Length;
            }
            button_code.IsEnabled = !System.String.IsNullOrWhiteSpace(code1.Text);

        }
      
        private void button_code_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(code1.Text) == code_sent)
            {
                logBox.AppendText("\nSaving new data\n\n");
                System.IO.StreamWriter file2 = new System.IO.StreamWriter("data.txt");
                lines = phoneBox.Text + "\r\n" + bankBox.Text + "\r\n" + firstnameBox.Text + "\r\n" + lastnameBox.Text + "\r\n" + emailBox.Text;
                file2.Write(lines);
                file2.Close();
                logBox.AppendText("\nData saved\n\n ....................................");
                code_sent = 0;
            }
            else logBox.AppendText("\nWrong code inserted \n ...................................");
            logBox.ScrollToEnd();
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }
    }
}
