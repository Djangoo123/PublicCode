using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.Interface
{
    public interface IOffice
    {
        public List<DeskModel> OfficeData();
        public List<DeskModel> UpdateOfficeData(List<DeskModel> model);
        public DataOffice DuplicateDesk(DataOffice model);
        public List<DeskModel> CreateAMap(MapCreationModel model);
        public DataOffice CreateDeskSeparator(string designationName);
        public int DeleteDesk(int Id);
    }
}
