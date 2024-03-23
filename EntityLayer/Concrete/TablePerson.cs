using EntityLayer.Abstract;
using System;
using System.Collections.Generic;

namespace deviceInterfacev2.Models;

public partial class TablePerson : IDatabaseEntity
{
    public int PersonId { get; set; }

    public string? PersonName { get; set; }

    public string? PersonSurname { get; set; }

    public string? PersonPhone { get; set; }

    public string? PersonMail { get; set; }

    public int PersonUserId { get; set; }

    public int PersonContactId { get; set; }

    public virtual TableContact PersonContact { get; set; } = null!;

    public virtual TableUser PersonUser { get; set; } = null!;
}
