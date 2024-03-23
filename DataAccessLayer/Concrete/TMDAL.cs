using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer.Concrete
{
    public class TMDAL : EfRepositoryBase<TableTm, TempDatabaseContext>
    {
        public List<ReportModel> getLastMonthValues(string unique_key)
        {
           
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                List<ReportModel> mylist = new List<ReportModel>();
                var minDate = DateTime.Now.AddMonths(-1);
                var device_id = my_context.TableDevices.Where(a=>a.DeviceUniqeKey== unique_key).FirstOrDefault().DeviceId;
                var list_tms = my_context.TableTMS.Where(a=>a.TmdeviceId== device_id && a.TMDate>minDate && (a.TMDate.Value.Hour==9 || a.TMDate.Value.Hour == 13 || a.TMDate.Value.Hour == 16)).Distinct().OrderByDescending(a=>a.TMDate).Reverse().ToList();
                var list_formaxmin = my_context.TableTMS.Where(a=>a.TmdeviceId== device_id && a.TMDate>minDate).Distinct().ToList();
                var date_list = list_tms.Select(a => a.TMDate.Value.Date).Distinct();
                foreach (var mydate in date_list)
                {
                    ReportModel rm = new ReportModel();
                    rm.date = mydate;
                    var dated_list_formaxmin = list_formaxmin.Where(a => a.TMDate.Value.Date == mydate);

                    List<double?> temps = new List<double?>();
                    List<double?> moistures = new List<double?>();
                    foreach (var maxmin in dated_list_formaxmin)
                    {
                        temps.Add(maxmin.Temp1);
                        temps.Add(maxmin.Temp2);
                        temps.Add(maxmin.Temp3);
                        temps.Add(maxmin.Temp4);
                        temps.Add(maxmin.Temp5);
                        temps.Add(maxmin.Temp6);
                        moistures.Add(maxmin.Moisture1);
                        moistures.Add(maxmin.Moisture2);
                    }

                    rm.maxt = temps.Max();
                    rm.mint = temps.Min();
                    rm.minm = moistures.Min();
                    rm.maxm= moistures.Max();

                    var first_nine = list_tms.Where(a => a.TMDate.Value.Hour == 9 && a.TMDate.Value.Date== mydate).OrderByDescending(a=>a.TMDate).Reverse().FirstOrDefault();
                    var first_thirteen = list_tms.Where(a => a.TMDate.Value.Hour == 13 && a.TMDate.Value.Date == mydate).OrderByDescending(a=>a.TMDate).Reverse().FirstOrDefault();
                    var first_sixteen= list_tms.Where(a => a.TMDate.Value.Hour == 16 && a.TMDate.Value.Date == mydate).OrderByDescending(a=>a.TMDate).Reverse().FirstOrDefault();

                    if (first_nine!=null)
                    {
                        rm.t9 = first_nine.Temp1;
                        rm.m9 = first_nine.Moisture1;
                    }
                    else if (first_thirteen!=null)
                    {
                        rm.t13 = first_thirteen.Temp1;
                        rm.m13 = first_thirteen.Moisture1;
                    }
                    else if (first_sixteen!=null)
                    {

                        rm.t16 = first_sixteen.Temp1;
                        rm.m16 = first_sixteen.Moisture1;
                    }
                    mylist.Add(rm);
                }

                return mylist;
            }
        }
    }

}
