using EntityLayer.Abstract;
using System;
using System.Collections.Generic;

namespace deviceInterfacev2.Models;

public partial class TableContact : IDatabaseEntity
{
    public int ContactId { get; set; }

    public string? ContactFirmName { get; set; }

    public string? ContactFirmAdress { get; set; }

    public string? ContactTc { get; set; }

    public int ContactCity { get; set; }

    public int ContactCounty { get; set; }

    public string? ContactTaxLoc { get; set; }

    public string? ContactTaxNo { get; set; }

    public int ContactUserId { get; set; }

    public virtual TableUser ContactUser { get; set; } = null!;

    public virtual ICollection<TablePerson> TablePeople { get; } = new List<TablePerson>();
}
