using EntityLayer.Abstract;
using System;
using System.Collections.Generic;

namespace deviceInterfacev2.Models;

public partial class TableCounty : IDatabaseEntity
{
    public int CountyId { get; set; }

    public string CountyName { get; set; } = null!;

    public int CountyNumber { get; set; }

    public int CountyCityId { get; set; }

    public virtual TableCity CountyCity { get; set; } = null!;
}
