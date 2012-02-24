using System.Web.Profile;
using System.Web.Security;
using MongoDB.Bson;
using MovieCatalog.Models.Interfaces;

namespace MovieCatalog.Models
{
    public class MembershipProvider<T> : System.Web.Security.MembershipProvider
        where T : System.Web.Security.MembershipProvider, new()
    {
        private readonly T _wrappedProvider;

        public MembershipProvider()
        {
            var a = new SqlMembershipProvider();
            _wrappedProvider = new T();
            _wrappedProvider.ValidatingPassword += WrappedProvider_ValidatingPassword;
        }

        #region Deligation of interface methoda

        private void WrappedProvider_ValidatingPassword(object sender, ValidatePasswordEventArgs e)
        {
            OnValidatingPassword(e);
        }

        public override string Name
        {
            get { return _wrappedProvider.Name; }
        }

        public override string Description
        {
            get { return _wrappedProvider.Description; }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _wrappedProvider.Initialize(name, config);
        }

        public override string ToString()
        {
            return _wrappedProvider.ToString();
        }

        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {

            var membershipUser = _wrappedProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer,
                                                             isApproved,
                                                             providerUserKey, out status);

            if (status == MembershipCreateStatus.Success)
            {
                OnUserSuccessfulCreated( membershipUser, ref status );
            }
            return membershipUser;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            return _wrappedProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion,
                                                                    newPasswordAnswer);
        }

        public override string GetPassword(string username, string answer)
        {
            return _wrappedProvider.GetPassword(username, answer);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return _wrappedProvider.ChangePassword(username, oldPassword, newPassword);
        }

        public override string ResetPassword(string username, string answer)
        {
            return _wrappedProvider.ResetPassword(username, answer);
        }

        public override void UpdateUser(MembershipUser user)
        {
            _wrappedProvider.UpdateUser(user);
        }

        public override bool ValidateUser(string username, string password)
        {
            return _wrappedProvider.ValidateUser(username, password);
        }

        public override bool UnlockUser(string userName)
        {
            return _wrappedProvider.UnlockUser(userName);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return _wrappedProvider.GetUser(providerUserKey, userIsOnline);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return _wrappedProvider.GetUser(username, userIsOnline);
        }

        public override string GetUserNameByEmail(string email)
        {
            return _wrappedProvider.GetUserNameByEmail(email);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return _wrappedProvider.DeleteUser(username, deleteAllRelatedData);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return _wrappedProvider.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline()
        {
            return _wrappedProvider.GetNumberOfUsersOnline();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            return _wrappedProvider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            return _wrappedProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override bool EnablePasswordRetrieval
        {
            get { return _wrappedProvider.EnablePasswordRetrieval; }
        }

        public override bool EnablePasswordReset
        {
            get { return _wrappedProvider.EnablePasswordReset; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _wrappedProvider.RequiresQuestionAndAnswer; }
        }

        public override string ApplicationName
        {
            get { return _wrappedProvider.ApplicationName; }
            set { _wrappedProvider.ApplicationName = value; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _wrappedProvider.MaxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _wrappedProvider.PasswordAttemptWindow; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _wrappedProvider.RequiresUniqueEmail; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _wrappedProvider.PasswordFormat; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _wrappedProvider.MinRequiredPasswordLength; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _wrappedProvider.MinRequiredNonAlphanumericCharacters; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _wrappedProvider.PasswordStrengthRegularExpression; }
        }

        #endregion

        private static void OnUserSuccessfulCreated( MembershipUser membershipUser, ref MembershipCreateStatus status )
        {
            //IRepository repository = RepositoryFactory.GetRepository();
            //User user = new User
            //                {
            //                    Id = new ObjectId(),
            //                    Name = membershipUser.UserName,
            //                };

            //repository.AddUser( user );

            //var profile = ProfileBase.Create( user.Name );
            //profile.SetPropertyValue( "UserId", user.Id );
            //profile.Save();
        }

    }
}