using Trident.Domain;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Trident.Contracts
{
    /// <summary>
    /// Interface for a general purpose email service
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the mail asynchronous give an email templated.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="to">To.</param>
        /// <param name="model">The model.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="viewBag">The view bag.</param>
        /// <param name="replyTo">Converts to .</param>
        /// <param name="from">From.</param>
        /// <param name="bcc">The BCC.</param>
        /// <param name="modelType">Type of the model.</param>
        void SendMailAsync(EmailTemplate template, 
            string to, object model, string subject = null, 
            List<Attachment> attachments = null, 
            object viewBag = null,
            MailAddress replyTo = null, 
            MailAddress from = null, 
            MailAddress bcc = null, 
            Type modelType = null);


        /// <summary>
        /// Sends the mail asynchronous given an html encoded message string.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="messageHtml">The message HTML.</param>
        /// <param name="to">To.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="messagePlainText">The message plain text.</param>
        /// <param name="replyTo">Converts to .</param>
        /// <param name="from">From.</param>
        /// <param name="bcc">The BCC.</param>
        void SendMailAsync(string subject, string messageHtml, string to, 
            List<Attachment> attachments = null, 
            string messagePlainText = null, 
            MailAddress replyTo = null, 
            MailAddress from = null, 
            MailAddress bcc = null);

    }
}
