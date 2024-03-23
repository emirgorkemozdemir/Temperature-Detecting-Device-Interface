using DataAccessLayer.Abstract;
using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer.Concrete
{
    public class UserDAL : EfRepositoryBase<TableUser, TempDatabaseContext>
    {
        public string UserRegisterExistingCheckDAL(TableUser my_user)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                string return_text = "";
                var selected_userMail = my_context.TableUsers.FirstOrDefault(i => i.UserMail == my_user.UserMail);

                if (selected_userMail != null)
                {
                    return_text = "Girdiğiniz mail adresi kullanılmaktadır.";
                }

                return return_text;
            }
        }

        public TableUser UserLoginDAL(TableUser my_user)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {

                var selected_user = my_context.TableUsers.FirstOrDefault(i => i.UserMail == my_user.UserMail && i.UserPassword == my_user.UserPassword);
                return selected_user;
            }
        }

        public Dictionary<string, int> isUserCompletedContact(string user_mail)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var user_id = my_context.TableUsers.FirstOrDefault(a => a.UserMail == user_mail).UserId;
                var selected_contact = my_context.TableContacts.FirstOrDefault(a => a.ContactUserId == user_id);
                Dictionary<string, int> mydict = new Dictionary<string, int>();
                if (selected_contact == null)
                {
                    mydict.Add("AccountType", user_id);
                    return mydict;
                }
                else
                {
                    mydict.Add("MainPage", user_id);
                    return mydict;
                }
            }
        }

        public Dictionary<List<string>, List<int>> isUserCompletedPersonInfos(string user_mail)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                Dictionary<List<string>, List<int>> mydict = new Dictionary<List<string>, List<int>>();
                List<string> my_string_list = new List<string>();
                List<int> my_int_list = new List<int>();


                var user = my_context.TableUsers.FirstOrDefault(a => a.UserMail == user_mail);
                var selected_contact = my_context.TableContacts.FirstOrDefault(a => a.ContactUserId == user.UserId);
                var selected_persons = my_context.TablePersons.Where(a => a.PersonUserId == user.UserId).ToList();
                if (selected_persons.Count() == 0)
                {
                    if (selected_contact == null)
                    {
                        my_string_list.Add("GoToContact");
                        my_string_list.Add(user_mail);
                        my_int_list.Add(user.UserId);


                        mydict.Add(my_string_list, my_int_list);

                        return mydict;
                    }
                    else if (selected_contact.ContactTc == null)
                    {
                        my_string_list.Add("FirmPersonInfos");
                        my_string_list.Add(user_mail);
                        my_int_list.Add(user.UserId);
                        my_int_list.Add(selected_contact.ContactId);


                        mydict.Add(my_string_list, my_int_list);

                        return mydict;
                    }
                    else
                    {
                        my_string_list.Add("NotCompletedIndividualPersonInfos");
                        my_string_list.Add(user_mail);

                        my_string_list.Add(user_mail);
                        my_int_list.Add(user.UserId);
                        my_int_list.Add(selected_contact.ContactId);

                        mydict.Add(my_string_list, my_int_list);

                        return mydict;

                    }

                }
                else
                {
                    my_string_list.Add("MainPage");
                    my_string_list.Add(user_mail);
                    my_int_list.Add(user.UserId);
                    my_int_list.Add(selected_contact.ContactId);

                    mydict.Add(my_string_list, my_int_list);

                    return mydict;

                }
            }
        }

        public TableContact IsIndividualContactExisting(string contact_tc)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_contact = my_context.TableContacts.FirstOrDefault(a => a.ContactTc == contact_tc);
                return selected_contact;
            }
        }

        public TableContact IsFirmContactExisting(string tax_no)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_contact = my_context.TableContacts.FirstOrDefault(a => a.ContactTaxNo == tax_no);
                return selected_contact;
            }
        }

        public int IsInfosExisting(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_persons = my_context.TablePersons.Where(a => a.PersonUserId == user_id).ToList();
                return selected_persons.Count();
            }
        }

        public void ChangePassword(string mail_adress, string new_password)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_user = my_context.TableUsers.FirstOrDefault(a => a.UserMail == mail_adress);
                selected_user.UserPassword = new_password;
                my_context.SaveChanges();
            }

        }

        public TableUser getUserByID(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_user = my_context.TableUsers.Find(user_id);
                return selected_user;
            }

        }

        public TableUser IsUserAdmin(TableUser my_user)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var admin = my_context.TableUsers.FirstOrDefault(a=>a.UserMail== my_user.UserMail && my_user.UserMail=="adminbiocoder@gmail.com" && a.UserPassword==my_user.UserPassword);
                return admin;
            }
        }


        //public List<string> MergedInfos(int user_id)
        //{
        //    List<string> mergedInfoList = new List<string>();
        //    using (TempDatabaseContext my_context = new TempDatabaseContext())
        //    {
        //        var selected_user = my_context.TableUsers.FirstOrDefault(i => i.UserId == user_id);
        //        var selected_person = my_context.TablePeople.FirstOrDefault(i => i.PersonMail1 == selected_user.UserMail);

        //        var selected_city = my_context.TableCities.FirstOrDefault(i => i.CityId == selected_user.UserFirmCity);
        //        var selected_county = my_context.TableCounties.FirstOrDefault(i => i.CountyId == selected_user.UserFirmCounty);

        //        mergedInfoList.Add(selected_person.PersonName1);
        //        mergedInfoList.Add(selected_person.PersonSurname1);
        //        mergedInfoList.Add(selected_person.PersonPhone1);
        //        mergedInfoList.Add(selected_person.PersonPhone2);
        //        mergedInfoList.Add(selected_person.PersonPhone3);
        //        mergedInfoList.Add(selected_person.PersonMail1);
        //        mergedInfoList.Add(selected_user.UserFirmName);
        //        mergedInfoList.Add(selected_user.UserFirmAdress);
        //        mergedInfoList.Add(selected_city.CityName);
        //        mergedInfoList.Add(selected_county.CountyName);
        //    }

        //    return mergedInfoList;

        //}


    }
}
