using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class DeviceDAL : EfRepositoryBase<TableDevice, TempDatabaseContext>
    {
        public TableDevice IsDeviceExisting(string uniqe_key)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_device = my_context.TableDevices.FirstOrDefault(i => i.DeviceUniqeKey == uniqe_key);
                return selected_device;
            }
        }

        public List<TableDevice> ListAllDevices(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_devices = my_context.TableDevices.Where(a => a.DeviceConnectedUser == user_id).ToList();
                return selected_devices;
            }
        }


        public List<Dictionary<List<string>, List<double?>>> ListAllDevicesWithClosestValues(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_devices = my_context.TableDevices.Where(a => a.DeviceConnectedUser == user_id).ToList();
                List<Dictionary<List<string>, List<double?>>> great_list = new List<Dictionary<List<string>, List<double?>>>();
                foreach (var device in selected_devices)
                {
                    Dictionary<List<string>, List<double?>> device_infos = new Dictionary<List<string>, List<double?>>();
                    var tmlist_devices = my_context.TableTMS.Where(a => a.TmdeviceId == device.DeviceId).OrderByDescending(a => a.TMDate).ToList();
                    double? c_t1 = null, c_t2 = null, c_t3 = null, c_t4 = null, c_t5 = null, c_t6 = null, c_h1 = null, c_h2 = null;
                    List<double?> list = new List<double?>();

                    foreach (var tmlist_device in tmlist_devices)
                    {
                        if (tmlist_device.Temp1 != null && c_t1 == null)
                        {
                            c_t1 = (double)tmlist_device.Temp1;
                            continue;
                        }
                        else if (tmlist_device.Temp2 != null && c_t2 == null)
                        {
                            c_t2 = (double)tmlist_device.Temp2;
                            continue;
                        }
                        else if (tmlist_device.Temp3 != null && c_t3 == null)
                        {
                            c_t3 = (double)tmlist_device.Temp3;
                            continue;

                        }
                        else if (tmlist_device.Temp4 != null && c_t4 == null)
                        {
                            c_t4 = (double)tmlist_device.Temp4;
                            continue;
                        }
                        else if (tmlist_device.Temp5 != null && c_t5 == null)
                        {
                            c_t5 = (double)tmlist_device.Temp5;
                            continue;
                        }
                        else if (tmlist_device.Temp6 != null && c_t6 == null)
                        {
                            c_t6 = (double)tmlist_device.Temp6;
                            continue;
                        }
                        else if (tmlist_device.Moisture1 != null && c_h1 == null)
                        {
                            c_h1 = (double)tmlist_device.Moisture1;
                            continue;
                        }
                        else if (tmlist_device.Moisture1 != null && c_h2 == null)
                        {
                            c_h2 = (double)tmlist_device.Moisture2;
                            continue;
                        }
                    }

                    List<string> list_string = new List<string>();
                    list_string.Add(device.DeviceName);
                    list_string.Add(device.DeviceUniqeKey);

                    list.Add(c_t1);
                    list.Add(c_t2);
                    list.Add(c_t3);
                    list.Add(c_t4);
                    list.Add(c_t5);
                    list.Add(c_t6);
                    list.Add(c_h1);
                    list.Add(c_h2);

                    device_infos.Add(list_string, list);
                    great_list.Add(device_infos);
                }
                return great_list;
            }
        }

        public int findIDbyUniqeKey(string unique_key)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_device_id = my_context.TableDevices.FirstOrDefault(a => a.DeviceUniqeKey == unique_key).DeviceId;
                return selected_device_id;
            }
        }

        public TableDevice findDevicebyUniqeKey(string unique_key)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_device = my_context.TableDevices.FirstOrDefault(a => a.DeviceUniqeKey == unique_key);
                return selected_device;
            }
        }

        public void deleteDeviceByUniqueKey(string unique_key)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_device = my_context.TableDevices.FirstOrDefault(a => a.DeviceUniqeKey == unique_key);
                my_context.TableDevices.Remove(selected_device);
                my_context.SaveChanges();
            }
        }

        public DateTime? getDeviceLastTM(int device_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_date = my_context.TableTMS.Where(a => a.TmdeviceId == device_id).OrderByDescending(a => a.TMDate).FirstOrDefault();

                return selected_date.TMDate;

            }

        }

        public List<double?> GetTMValues(int device_id,string key)
        {
        
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                List<double?> xx = new List<double?>();


                if (key=="t1")
                {
                    var t1list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Temp1 != null).Select(a => a.Temp1).ToList();
                    return t1list;
                }
                else if (key == "t2")
                {
                    var t2list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Temp2 != null).Select(a => a.Temp2).ToList();
                    return t2list;
                }
                else if (key == "t3")
                {
                    var t3list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Temp3 != null).Select(a => a.Temp3).ToList();
                    return t3list;
                }
                else if (key == "t4")
                {
                    var t4list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Temp4 != null).Select(a => a.Temp4).ToList();
                    return t4list;
                }
                else if (key == "t5")
                {
                    var t5list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Temp5 != null).Select(a => a.Temp5).ToList();
                    return t5list;
                }
                else if (key == "t6")
                {
                    var t6list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Temp6 != null).Select(a => a.Temp6).ToList();
                    return t6list;
                }
                else if (key == "h1")
                {
                    var m1list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Moisture1 != null).Select(a => a.Moisture1).ToList();
                    return m1list;
                }
                else if (key == "h2")
                {
                    var m2list = my_context.TableTMS.Where(a => a.TmdeviceId == device_id && a.Moisture2 != null).Select(a => a.Moisture2).ToList();
                    return m2list;
                }
                else
                {
                    return xx;
                }
            }

            
        }
    }
}
