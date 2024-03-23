using EntityLayer.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace deviceInterfacev2.Models;

public partial class TableUser : IDatabaseEntity
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Email boş bırakılamaz.")]
    [StringLength(40, MinimumLength = 10, ErrorMessage = "Mail adresi en az 10 en fazla 40 karakter olabilir.")]
    [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
    public string UserMail { get; set; } = null!;

    [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
    [StringLength(600, MinimumLength = 8, ErrorMessage = "Şifre en az 8 karakter olabilir.")]
    public string UserPassword { get; set; } = null!;

    public DateTime? UserRegisterDate { get; set; }

    public DateTime? UserConfigurateDate { get; set; }

    public virtual ICollection<TableContact> TableContacts { get; } = new List<TableContact>();

    public virtual ICollection<TablePerson> TablePeople { get; } = new List<TablePerson>();

    public virtual ICollection<TableDevice> TableDevices { get; } = new List<TableDevice>();
}
