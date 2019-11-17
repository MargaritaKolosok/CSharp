using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Configuration;
using Common.Enums;
using HtmlAgilityPack;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace Common.Managers
{
    /// <summary>
    /// Work with IMAP mail server, mailboxes, and mail content
    /// </summary>
    public class MailManager : IDisposable
    {
        public ImapClient ClientCxM;
        public ImapClient ClientUserDirectory;
        private static Dictionary<UniqueId, MimeMessage> _messagesList;
        private static IList<UniqueId> _messageUidList = new List<UniqueId>();
        private const string ActivationSubject = "activ";
        public const string PasswordResetSubject = "new password";
        public const string RegistrationConfirmationSubject = "confirm";
        private const string AccessRequestSubject = "requested access";
        public const string NewUserRegisteredSubject = "new user has registered";

        /// <summary>
        /// Establishes connection to mail server depending on user type and creates  
        /// a new <see cref="T:MailKit.Net.Imap.ImapClient" /> instance
        /// </summary>
        /// <param name="userType">User type (see <see cref="T:Common.Enums.UserType"/>)</param>
        public Task ConnectMailServerAsync(UserType userType)
        {
            return Task.Run(() =>
            {
                switch (userType)
                {
                    case UserType.AdminCxM:
                        ClientCxM?.Dispose();
                        ClientCxM = new ImapClient
                        {
                            ServerCertificateValidationCallback = (s, c, h, e) => true
                        };

                        try
                        {
                            ClientCxM.Connect(TestConfig.MailServerDomain, TestConfig.MailServerPort);
                            ClientCxM.Authenticate(TestConfig.MailServerLogin, TestConfig.MailServerPassword);
                            ClientCxM.Inbox.Open(FolderAccess.ReadWrite);
                        }
                        catch (Exception ex)
                        {
                            throw new AggregateException($"Mail server error: {ex.Message}");
                        }

                        break;
                    case UserType.AdminUserDirectory:
                        ClientUserDirectory?.Dispose();
                        ClientUserDirectory = new ImapClient
                        {
                            ServerCertificateValidationCallback = (s, c, h, e) => true
                        };
                        try
                        {
                            ClientUserDirectory.Connect(TestConfig.MailServerDomainAdmin,
                                TestConfig.MailServerPortAdmin);
                            ClientUserDirectory.Authenticate(TestConfig.MailServerLoginAdmin,
                                TestConfig.MailServerPasswordAdmin);
                            ClientUserDirectory.Inbox.Open(FolderAccess.ReadWrite);
                        }
                        catch (Exception ex)
                        {
                            throw new AggregateException($"Mail server error: {ex.Message}");
                        }

                        break;
                }
            });
        }

        /// <summary>
        /// Restores connection to <see cref="T:MailKit.Net.Imap.ImapClient"/> instance
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient"/> instance</param>
        private void Reconnect(ImapClient client)
        {
            ConnectMailServerAsync(client == ClientCxM ? UserType.AdminCxM : UserType.AdminUserDirectory)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Requests <see cref="T:MailKit.Net.Imap.ImapClient" /> for a new mail. Unread emails will be 
        /// ordered by date in descending order and stored in _messagesList.
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        /// <returns>(<see cref="bool"/>) True if new mail arrived</returns>
        public static bool GotNewMail(ImapClient client)
        {
            _messageUidList = client.Inbox
                .Search(SearchQuery.NotSeen)
                .OrderByDescending(x => OrderBy.Date)
                .ToArray();

            _messagesList = new Dictionary<UniqueId, MimeMessage>();
            foreach (var uid in _messageUidList)
            {
                _messagesList.Add(uid, client.Inbox.GetMessage(uid));
            }

            return _messagesList.Count > 0;
        }

        /// <summary>
        /// Determines whether a registration confirmation email has been received
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if received</returns>
        public static bool IsConfirmationEmailReceived()
        {
            return _messagesList.Any(x => x.Value.Subject.Contains(RegistrationConfirmationSubject));
        }

        /// <summary>
        /// Determines whether a new access permissions request email has been received
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if received</returns>
        public bool IsAccessRequestEmailReceived()
        {
            return _messagesList.Any(x => x.Value.Subject.Contains(AccessRequestSubject));
        }

        /// <summary>
        /// Determines whether a user password reset email has been received
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if received</returns>
        public static bool IsPasswordResetEmailReceived()
        {
            return _messagesList.Any(x => x.Value.Subject.Contains(PasswordResetSubject));
        }

        /// <summary>
        /// Determines whether a user activation email has been received
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if received</returns>
        public bool IsUserActivationEmailReceived()
        {
            return _messagesList.Any(x => x.Value.Subject.Contains(ActivationSubject));
        }

        /// <summary>
        /// Extracts HTTP URL from email body
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        /// <param name="emailSubjectText">Subject text to find in emails</param>
        /// <returns>(<see cref="string"/>) URL string</returns>
        public string GetLinkFromEmail(ImapClient client, string emailSubjectText)
        {
            const string stringToFind = "http";
            var email = _messagesList
                .FirstOrDefault(x => x.Value.Subject.Contains(emailSubjectText));

            if (email.Key.Id == 0)
            {
                return null;
            }

            client.Inbox.AddFlags(_messageUidList, MessageFlags.Seen, silent: true);
            var ma = Regex.Matches(email.Value.HtmlBody, "<a.+?href=['\"](?<href>.*?)['\"].*?>(?<txt>.+?)</a>", 
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var link = ma.OfType<Match>()
                .Select(m => new {Href = m.Groups["href"].Value, InnerHtml = m.Groups["txt"].Value})
                .Select(m => m.Href)
                .FirstOrDefault();
            if (link != null)
            {
                return link.Trim();
            }

            var textBody = GetPlainText(email.Value);
            var position = textBody
                .IndexOf(stringToFind, StringComparison.OrdinalIgnoreCase);
            if (position < 0)
            {
                return null;
            }
            var pieceWithLink = textBody.Substring(position).TrimStart();
            position = pieceWithLink.IndexOf(" ", StringComparison.Ordinal);

            return position < 0
                ? pieceWithLink
                : pieceWithLink.Substring(0, position);
        }

        /// <summary>
        /// Extracts password from specified place in user password reset email
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        /// <param name="emailSubjectText">Subject text to find in emails</param>
        /// <returns>(<see cref="string"/>) Password string</returns>
        public string GetPasswordFromEmail(ImapClient client, string emailSubjectText)
        {
            const string stringToFind = "to log in: ";
            var email = _messagesList.OrderByDescending(x => x.Key)
                .FirstOrDefault(x => x.Value.Subject.Contains(emailSubjectText));

            if (uint.Parse(email.Key.ToString()) == 0)
            {
                return null;
            }

            client.Inbox.AddFlags(_messageUidList, MessageFlags.Seen, silent: true);
            var textBody = GetPlainText(email.Value);
            var position = textBody
                .IndexOf(stringToFind, StringComparison.OrdinalIgnoreCase);
            if (position < 0)
            {
                return string.Empty;
            }
            var pieceWithPassword = textBody
                .Substring(position + stringToFind.Length)
                .TrimStart();
            position = pieceWithPassword.IndexOf(' ');

            return position < 0
                ? pieceWithPassword
                : pieceWithPassword.Substring(0, position);
        }

        /// <summary>
        /// Returns email sender's information
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        /// <param name="emailSubjectText">Subject text to find in emails</param>
        /// <returns>(<see cref="string"/>) Sender's name and email string</returns>
        public string GetSenderFromEmail(ImapClient client, string emailSubjectText)
        {
            var email = _messagesList
                .OrderByDescending(x => x.Key)
                .FirstOrDefault(x => x.Value.Subject == emailSubjectText);

            if (email.Key.Id == 0)
            {
                return null;
            }
            client.Inbox.AddFlags(email.Key, MessageFlags.Seen, silent: true);
            var addresses = string.Join(", ", email.Value.From);
            return addresses;
        }

        /// <summary>
        /// Determines whether a message body contains text
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        /// <param name="emailSubjectText">Subject text to find in emails</param>
        /// <param name="textToFind">Text to find in message body</param>
        /// <param name="parseAsIs">Do not extract plain text from HTML body</param>
        /// <returns>(<see cref="bool"/>) True if contains</returns>
        public bool? IsMailBodyContainsText(
            ImapClient client, string emailSubjectText, string textToFind, bool parseAsIs = false)
        {
            var emails = _messagesList
                .OrderByDescending(x => x.Key)
                .Where(x => x.Value.Subject == emailSubjectText)
                .ToArray();

            if (emails.Length == 0 || emails.Any(x => x.Key.Id == 0))
            {
                return null;
            }

            client.Inbox.AddFlags(emails.Select(x => x.Key).ToArray(), MessageFlags.Seen, silent: true);
            
            return emails
                .Select(email => parseAsIs ? email.Value.HtmlBody : GetPlainText(email.Value))
                .Any(body => body.Contains(textToFind));
        }

        /// <summary>
        /// Decodes HTML and plain text emails body to plain text
        /// </summary>
        /// <param name="email">Email message object</param>
        /// <returns>(<see cref="string"/>) Email body plain text</returns>
        private static string GetPlainText(MimeMessage email)
        {
            var textBody = email.TextBody;
            
            if (!string.IsNullOrEmpty(textBody))
            {
                return textBody;
            }

            var unparsedHtml = email.HtmlBody;
            var body = new HtmlDocument();
            body.LoadHtml(unparsedHtml);
            var decodedText = WebUtility.HtmlDecode(body.DocumentNode.InnerText);
            
            if (string.IsNullOrEmpty(decodedText))
            {
                return string.Empty;
            }

            textBody = string.Join(" ", Regex.Split(decodedText, @"\r\n\r\n"));
            textBody = string.Join(string.Empty, Regex.Split(textBody, @"(?:=\r\n|\r\n|\n|\r|\t)"));
            textBody = textBody.Replace("=3D", "=");
            return textBody;
        }

        /// <summary>
        /// Removes all emails from Inbox folder of specified <see
        /// cref="T:MailKit.Net.Imap.ImapClient" /> instance 
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        public void InboxHousekeeping(ImapClient client)
        {
            InboxHousekeepingAsync(client).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Removes all emails from Inbox folder asynchronously
        /// </summary>
        /// <param name="client"><see cref="T:MailKit.Net.Imap.ImapClient" /> instance</param>
        public async Task InboxHousekeepingAsync(ImapClient client)
        {
            if (!client.IsConnected)
            {
                Reconnect(client);
            }
            var messageUidList = client.Inbox.Search(SearchQuery.All);
            if (messageUidList == null || messageUidList.Count <= 0)
            {
                return;
            }
            await client.Inbox.AddFlagsAsync(messageUidList, MessageFlags.Deleted, silent: true)
                .ContinueWith(task => client.Inbox.ExpungeAsync(), TaskContinuationOptions.AttachedToParent);
        }

        ~MailManager()
        {
            Dispose(false);
        }

        private void Dispose(bool isDispose)
        {
            if (isDispose)
            {
                ClientCxM?.Dispose();
                ClientUserDirectory?.Dispose();
            }
            _messageUidList = null;
            _messagesList = null;
        }
        
        /// <summary>
        /// Closes clients connections and disposes class resources 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
