import { useEffect, useState } from "react";

export default function TeachersList() {
    const [teachers, setTeachers] = useState([]);

    useEffect(() => {
        fetch("https://localhost:5001/api/teachers")
            .then((res) => res.json())
            .then((data) => setTeachers(data));
    }, []);

    return (
        <div>
            <h2>Lista de Profesores</h2>
            <ul>
                {teachers.map((t) => (
                    <li key={t.id}>
                        {t.firstName} {t.lastName} - {t.email}
                    </li>
                ))}
            </ul>
        </div>
    );
}
