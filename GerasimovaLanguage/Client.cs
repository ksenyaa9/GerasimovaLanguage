//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GerasimovaLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            this.ClientService = new HashSet<ClientService>();
            this.Tag = new HashSet<Tag>();
        }
    
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string GenderCode { get; set; }

        public string Phone { get; set; }
        public string PhotoPath { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }

        public string BirthdayString
        {
            get
            {
                if (Birthday.HasValue)
                {
                    return Birthday.Value.ToShortDateString();
                }
                else
                    return "";
            }
        }

        public string Email { get; set; }
        public System.DateTime RegistrationDate { get; set; }

        public string RegistrationDateString
        {
            get
            {
                if (RegistrationDate != null)
                {
                    return RegistrationDate.ToShortDateString();
                }
                else
                    return "";
            }
        }


        public virtual Gender Gender { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientService> ClientService { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag> Tag { get; set; }

        public int VisitCount
        {
            get
            {
                var datelist = ClientService.Where(p => p.ClientID == this.ID).ToList();
                return datelist.Count;
            }
        }

        public DateTime? LastVisitDate
        {
            get
            {
                if (ClientService == null || !ClientService.Any())
                    return null;

                var latestService = ClientService.Where(p => p.ClientID == this.ID).OrderByDescending(p => p.StartTime).FirstOrDefault();
                return latestService?.StartTime;
            }
        }

        public string LastVisitDateString
        {
            get
            {
                return LastVisitDate.HasValue
                    ? LastVisitDate.Value.ToShortDateString()
                    : "нет";
            }
        }


    }
}
