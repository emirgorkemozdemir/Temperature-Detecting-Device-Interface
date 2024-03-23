using EntityLayer.Abstract;
using System;
using System.Collections.Generic;

namespace deviceInterfacev2.Models;

public partial class TableCity : IDatabaseEntity
{
    public int CityId { get; set; }

    public string CityPlate { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public virtual ICollection<TableCounty> TableCounties { get; } = new List<TableCounty>();
}
