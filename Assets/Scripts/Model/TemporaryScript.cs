using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if false
public class TemporaryScript 
{
    Task[] taskListActual = {
                    Task.ConvertingElements(new[]{"������ 1", "120","21.06.2022","12:00","5"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 4", "30","30.06.2022","17:00","8"})};

    Task[] taskListOverdue = {
                    Task.ConvertingElements(new[]{"������ 10", "120","11.06.2022","12:00","5"}),
                    Task.ConvertingElements(new[]{"������ 20", "60","4.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 30", "90","6.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 40", "30","7.06.2022","17:00","8"})};

    Task[] taskListWaiting = {
                    Task.ConvertingElements(new[]{"������ 100", "120","21.06.2022","12:00","5"}),
                    Task.ConvertingElements(new[]{"������ 200", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 300", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 400", "30","30.06.2022","17:00","8"})};

    Task[] taskListCompleted = {
                    Task.ConvertingElements(new[]{"������ 1000", "120","21.06.2022","12:00","5"}),
                    Task.ConvertingElements(new[]{"������ 2000", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3000", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 4000", "30","30.06.2022","17:00","8"})};
    
    // ��������
    public Task[] UpdateList()
    {
        if (taskListActual[0].name == "������ 1")
        {
            Task[] _taskListActual = {
            Task.ConvertingElements(new[] { "������ 1-1", "120", "21.06.2022", "12:00", "5" }),
            Task.ConvertingElements(new[] { "������ 1-2", "60", "24.06.2022", "14:00", "6" }),
            Task.ConvertingElements(new[] { "������ 1-3", "90", "27.06.2022", "16:00", "7" }),
            Task.ConvertingElements(new[] { "������ 1-4", "30", "30.06.2022", "17:00", "8" })};
            taskListActual = _taskListActual;
        }
        else
        {
            Task[] _taskListActual = {
                    Task.ConvertingElements(new[]{"������ 1", "120","21.06.2022","12:00","5"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 2", "60","24.06.2022","14:00","6"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 3", "90","27.06.2022","16:00","7"}),
                    Task.ConvertingElements(new[]{"������ 4", "30","30.06.2022","17:00","8"})};
            taskListActual = _taskListActual;
        }
        return taskListActual;
    }

    // ������� ������ ������
    public Task[] ReturnTaskList(int typeListTask)
    {
        switch (typeListTask)
        {
            case 0:
                return taskListActual;
            case 1:
                return taskListWaiting;
            case 2:
                return taskListOverdue;
            case 3:
                return taskListCompleted;
            default:
                return taskListActual;
        }
    }

    // ������� ������
    // �������� ������
    // ������������� ������

    // ������������� ��������
    // ������� ��������� ���������� 
    // �������� ����������� ������
    // ������� ���������� ���������� �������

    public int NewOverrideTask()
    {
        return 1;
    }

}

public class Task
{
    public string name;
    public int timeInMinutes;
    public DateTime dataDeadline;
    public byte importance;
    public DateTime beginning;
    public DateTime ending;
    public bool enoughTime = true;
    public bool @fixed = false;

    public Task() { }

    public Task(string name, int timeInMinutes, DateTime dataDeadline, byte importance, DateTime beginning, DateTime ending)
    {
        this.name = name;
        this.timeInMinutes = timeInMinutes;
        this.dataDeadline = dataDeadline;
        this.importance = importance;
        this.beginning = beginning;
        this.ending = ending;
    }

    public static Task ConvertingElements(string[] arrayStringElements)
    {
        Task task = new Task();

        task.name = arrayStringElements[0];
        task.timeInMinutes = int.Parse(arrayStringElements[1]);
        task.dataDeadline = Convert.ToDateTime(arrayStringElements[2]);
        task.dataDeadline = task.dataDeadline.AddHours((Convert.ToDateTime(arrayStringElements[3])).Hour);
        task.dataDeadline = task.dataDeadline.AddMinutes((Convert.ToDateTime(arrayStringElements[3])).Minute);
        task.importance = Convert.ToByte(arrayStringElements[4]);
        task.beginning = task.dataDeadline.AddMinutes(-task.timeInMinutes);
        task.ending = task.dataDeadline;

        return task;
    }
}

        
#endif