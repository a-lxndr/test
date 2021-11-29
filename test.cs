/*
I. Есть таблицы Users (Id, Name), UserRoles (UserId, RoleId), Roles (Id, Name).
a). Используя Entity Framework написать метод на получение пользователей, у которых есть определенная роль. Метод должен поддерживать постраничный вывод.  Параметры pageNumber, pageSize, role.
b). Написать этот же запрос, но с использованием SQL.
с). Доработать метод в пункте a).  чтобы получить не только пользователей, но и все их роли.
*/

public static void ShowUser(int pageNumber = 1, int pageSize = 1, string role="") {
    if (String.IsNullOrEmpty(role)) {
        Console.WriteLine("Не указана роль");
        return;
    }

    var _pageNumber = pageNumber > 0 ? pageNumber : 1;
    var _pageSize = pageSize > 0 ? pageSize : 1;

    using (appdbContext db = new appdbContext()) {
        var skipCount = (_pageNumber - 1) * _pageSize;
        var rolesID = db.Roles.Where(w => w.Name == role).Select(s => s.Id).ToList();
        var users = db.RoleUsers.Where(w => rolesID.Contains(w.RolesId)).Select(s => new
        {
            s.Users.Name,
            s.Users.Id,
            Roles = s.Users.RoleUsers.Select(s=>s.Roles.Name),
        })
            .Skip(skipCount)
            .Take(_pageSize);

        foreach (var user in users) {
            Console.WriteLine("Пользователь - " + user.Name);
            Console.Write("Роли: " + String.Join(", ", user.Roles));
            Console.WriteLine();
        }
    }
}

//sql
select u.Name, r.Name from RoleUser as ru
  inner join Users as u on u.Id = ru.UsersId
  inner join Roles as r on r.Id = ru.RolesId

where u.ID in (
    select u.ID from RoleUser as ru
    inner join Users as u on u.Id = ru.UsersId
    inner join Roles as r on r.Id = ru.RolesId
    where
    r.Name = 'admin'
)


/*
II. Есть таблица сотрудников Employees (Id, Name, ManagerId). Где ManagerId - ID руководителя. Таблица ссылается на себя.
Написать метод для получения списка подчинённых 1-ого и 2-ого уровня (прямые подчиненные и подчиненные прямых подчиненных) для произвольного сотрудника.
*/

public static void ShowManager(string name = "") {
    if (string.IsNullOrEmpty(name)) {
        Console.WriteLine("Не указано имя пользователя");
        return;
    }
    using (helloappdbContext db = new helloappdbContext()) {
        var users = db.Employees.Where(w => w.Name == name).Select(s => new
        {
            Name = s.Name,
            FirstLevel = s.InverseManager.Select(ss => ss.Name),
            SecondLevel = s.InverseManager.SelectMany(ss => ss.InverseManager.Select(sss=>sss.Name)),
        });
        foreach (var user in users) {
            Console.WriteLine(user.Name) ;
            Console.WriteLine("Первый уровень подчиненных");
            foreach (var firstlvl in user.FirstLevel) {
                Console.Write(firstlvl + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Второй уровень подчиненных");
            foreach (var secondlvl in user.SecondLevel) {
                Console.Write(secondlvl + " ");
            }
        }
    }
}

/*
III. Необходимо вставить 10000 записей в таблицу, в которой уже есть данные. Какими способами это можно сделать?
*/

List<User> users = new List<User>();
for (int i = 0; i < 100000; i++) {
    User user = new User { Name = "Smith "+i };
    users.Add(user);
}
using (appdbContext db = new appdbContext()) {
    db.Users.AddRange(users);
    db.SaveChanges();
}
