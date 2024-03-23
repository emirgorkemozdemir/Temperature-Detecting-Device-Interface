using DataAccessLayer.Concrete;
using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class DeviceBL : ManagerRepository<TableDevice, DeviceDAL>
    {
        DeviceDAL dd = new DeviceDAL();

        public TableDevice IsDeviceExistingBL(string uniqe_key)
        {
            return dd.IsDeviceExisting(uniqe_key);
        }

        public List<TableDevice> ListAllDevices(int user_id)
        {
            return dd.ListAllDevices(user_id);
        }

        public int findIDbyUniqeKey(string unique_key)
        {
            return dd.findIDbyUniqeKey(unique_key);
        }

        public List<Dictionary<List<string>, List<double?>>> ListAllDevicesWithClosestValues(int user_id)
        {
           return dd.ListAllDevicesWithClosestValues(user_id).ToList();
        }

        public void deleteDeviceByUniqueKey(string unique_key)
        {
            dd.deleteDeviceByUniqueKey(unique_key);
        }

        public TableDevice findDevicebyUniqeKey(string unique_key)
        {
            return dd.findDevicebyUniqeKey(unique_key);
        }

        public DateTime? getDeviceLastTM(int device_id)
        {
            return dd.getDeviceLastTM(device_id);

        }
        public List<double?> GetTMValues(int device_id, string key)
        {
            return dd.GetTMValues(device_id,key);
        }


    }
}
