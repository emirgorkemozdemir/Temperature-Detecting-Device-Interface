using EntityLayer.Abstract;
using System;
using System.Collections.Generic;

namespace deviceInterfacev2.Models;

public partial class TableTm : IDatabaseEntity
{
    public int Tmid { get; set; }

    public double? Temp1 { get; set; }

    public double? Temp2 { get; set; }

    public double? Temp3 { get; set; }

    public double? Temp4 { get; set; }

    public double? Temp5 { get; set; }

    public double? Temp6 { get; set; }

    public double? Moisture1 { get; set; }

    public double? Moisture2 { get; set; }

    public DateTime? TMDate { get; set; }

    public int TmdeviceId { get; set; }

    public virtual TableDevice Tmdevice { get; set; } = null!;
}
