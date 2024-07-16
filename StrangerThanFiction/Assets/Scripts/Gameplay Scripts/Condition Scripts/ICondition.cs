using System.Threading.Tasks;

public interface ICondition 
{
    Task OnAdd();
    Task OnTrigger();
    Task OnSurplus (ICondition surplus);
    Task OnRemove();
}
