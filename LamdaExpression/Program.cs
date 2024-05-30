namespace LamdaExpression;

public class Student
{
    public string? Name { get; set; }
    public int Grage { get; set; }
    public int Class { get; set; }
}

class Program
{
    static void HelloStudent(Action<Student> configureStudent)
    {
        var student = new Student();
        configureStudent(student);
        
        Console.WriteLine("Hello");
        Console.WriteLine(student.Name);
    }
    
    static void Main(string[] args)
    {
        HelloStudent(option =>
        {
            option.Name = "Nguyễn Khắc Thành";
            option.Class = 15;
            option.Grage = 15;
            Console.WriteLine("Cơm rang dưa bò");
        });
        
        //Tạm hiểu: Cho phép chèn một khối lệnh vào trong tham số của hàm, thực hiện từ trên xuống dưới.
    }
}