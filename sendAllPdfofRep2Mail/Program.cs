using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;
using System.IO;
using System.Xml.Serialization;

namespace sendAllPdfofRep2Mail
{


    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            DateTime dt = new DateTime();


            String AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] tabFiles = Directory.GetFileSystemEntries(AppDirectory, "*.pdf");

            if (tabFiles.Length == 0)
            {
                sb.Append(dt.ToString() + "Pas de mail à envoyer");
                Environment.Exit(0);
            }

            SendMailconfig smc = new SendMailconfig();
            XmlSerializer xs = new XmlSerializer(typeof(SendMailconfig));//pour serialiser en XML la config

            if (!File.Exists(AppDirectory + "\\sendMailConfig.xml"))//si le fichier n'existe pas on le cré avec init à "";
            {
                smc.destinataire = "destinataire@url.com";
                smc.CorpsDeMessage = "corps du message";
                smc.DestinataireEnCopie = "destinataire1@url.com;destinataire2@url.com";
                smc.Emetteur = "emetteur@url.com";
                smc.Objet = "Objet du mail";

                using (StreamWriter wr = new StreamWriter(AppDirectory + "\\sendMailConfig.xml"))
                {
                    xs.Serialize(wr, smc);//On sérialise;
                }
            }

            using (StreamReader rd = new StreamReader(AppDirectory + "\\sendMailConfig.xml"))
            {
                smc = xs.Deserialize(rd) as SendMailconfig;

            }



            //
            System.Net.Mail.Attachment attachment;
            MailMessage mail = new MailMessage();
            mail.Sender = new MailAddress("jcbillard@arterris.fr");
            mail.To.Add(new MailAddress("jcbillard@arterris.fr"));

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "vmexchange2010.sca.local";
            mail.Subject = "test Votre facture";
            mail.Body = "Corps du message";

            foreach (string item in tabFiles)
            {
                attachment = new System.Net.Mail.Attachment(item);
                mail.Attachments.Add(attachment);
                //un mail par facture
                //client.Send(mail);
                dt = DateTime.Now;
                sb.Append("mail envoyé le : " + dt.ToString()+ Environment.NewLine);
                File.AppendAllText(AppDirectory + "log.txt", sb.ToString());

            }

            try
            {
                client.Send(mail);
            }
            catch (SmtpFailedRecipientsException sfre )
            {

                throw;
            }
            catch (SmtpException se)
            {

                throw;
            }
            catch (ObjectDisposedException ode)
            {

                throw;
            }

           

        }
    }

    public class SendMailconfig
    {
        public SendMailconfig()
        {

        }

        public String destinataire = "";
        public String Emetteur = "";
        public String DestinataireEnCopie ="";
        public String Objet ="";
        public String CorpsDeMessage ="";
    }
}
