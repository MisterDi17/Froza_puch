using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IInteractionAction
{
    string ActionName { get; }            // Название (например, "Открыть", "Осмотреть", "Взломать") 
    float Duration { get; }               // Время выполнения (секунды)
    void Execute(GameObject player);      // Что происходит, когда действие завершилось
}