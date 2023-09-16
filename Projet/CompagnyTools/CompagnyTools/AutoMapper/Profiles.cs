using AutoMapper;
using CompagnyTools.Models;
using CompagnyTools.Models.SimpleModels;
using DAL.Entities;

namespace CompagnyTools.AutoMapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<DataOffice, OfficeModel>();
            CreateMap<OfficeModel, DataOffice>();

            CreateMap<Equipments, EquipmentsModel>();
            CreateMap<EquipmentsModel, Equipments>();

            CreateMap<Users, UserModel>();
            CreateMap<UserModel, Users>();
            
            CreateMap<Users, SimpleUserModel>();
            CreateMap<SimpleUserModel, Users>();

            CreateMap<UsersRoles, UsersRolesModel>();
            CreateMap<UsersRolesModel, UsersRoles>();
            
            CreateMap<UsersRoles, SimpleUserRolesModel>();
            CreateMap<SimpleUserRolesModel, UsersRoles>();
        }
    }
}
