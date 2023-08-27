using CompagnyTools.Models;

namespace CompagnyTools.Interface
{
    public interface IOffice
    {
        public List<DeskModel> OfficeData();
        public List<DeskModel> UpdateOfficeData(List<DeskModel> model);
        public List<DeskModel> CreateAMap(MapCreationModel model);
    }
}
