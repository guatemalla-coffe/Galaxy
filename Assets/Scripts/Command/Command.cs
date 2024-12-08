using System.Threading.Tasks;
using UnityEngine;

public abstract class Command
{
    public string commandName { get; protected set; }
    public BearController bear;
    public abstract Task ExecuteAsync();
    public abstract void Cancel();

}