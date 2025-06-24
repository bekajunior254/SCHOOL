import { useEffect, useState } from "react";
import axios from "axios";
import "./ManageTeachers.css";

export default function ManageTeachers() {
  const [teachers, setTeachers] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem("token");
    axios
      .get("http://localhost:5000/api/admin/teachers", {
        headers: { Authorization: `Bearer ${token}` },
      })
      .then((res) => setTeachers(res.data))
      .catch((err) => console.error("Error fetching teachers", err));
  }, []);

  return (
    <div className="manage-teachers-container">
      <h2>Manage Teachers</h2>
      <ul>
        {teachers.map((teacher) => (
          <li key={teacher.id}>
            {teacher.fullName} - {teacher.email}
          </li>
        ))}
      </ul>
    </div>
  );
}
