using System;
using TemplateIHD.Domain.Interfaces;

namespace TemplateIHD.Domain
{
    public class IHDEntity: IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
