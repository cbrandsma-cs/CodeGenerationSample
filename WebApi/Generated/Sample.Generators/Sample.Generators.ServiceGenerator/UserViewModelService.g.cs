using WebApi.Services.UserServices;
namespace WebApi.Services; 

public partial class UserViewModelService
{
    private static readonly List<UserViewModel> _list = new();
    public virtual List<UserViewModel> All()
    {
        return _list;
    }

    public virtual void Add(UserViewModel item)
    {
        _list.Add(item);
    }

    public virtual void Update(UserViewModel item)
    {
        var existing = _list.Single(x => x.Id == item.Id);
        _list.Remove(existing);
        _list.Add(item);
    }

    public virtual void Delete(int id)
    {
        var existing = _list.Single(x => x.Id == id);
        _list.Remove(existing);
    }
}