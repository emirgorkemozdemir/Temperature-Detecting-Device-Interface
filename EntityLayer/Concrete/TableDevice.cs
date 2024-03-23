using EntityLayer.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace deviceInterfacev2.Models;

public partial class TableDevice : IDatabaseEntity
{
    public int DeviceId { get; set; }

    public string? DeviceName{ get; set; } = null!;

    [Required(ErrorMessage = "Cihaz ID boş bırakılamaz.")]
    public string DeviceUniqeKey { get; set; } = null!;

    public int? DeviceConnectedUser { get; set; }
    public double? DeviceMaxTemp { get; set; }

    public double? DeviceMinTemp { get; set; }

    public double? DeviceMaxMoisture { get; set; }

    public double? DeviceMinMoisture { get; set; }

    public DateTime? DeviceRegisterDate { get; set; }


    public virtual TableUser DeviceConnectedUserNavigation { get; set; } = null!;

    public virtual ICollection<TableTm> TableTms { get; } = new List<TableTm>();
}
