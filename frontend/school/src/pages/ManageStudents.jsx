import { useEffect, useState } from "react";
import axios from "axios";
import "./ManageStudents.css";

export default function ManageStudents() {
  const [students, setStudents] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem("token");
    axios
      .get("http://localhost:5000/api/admin/students", {
        headers: { Authorization: `Bearer ${token}` },
      })
      .then((res) => setStudents(res.data))
      .catch((err) => console.error("Error fetching students", err));
  }, []);

  return (
    <div className="manage-students-container">
      <h2>Manage Students</h2>
      <ul>
        {students.map((student) => (
          <li key={student.id}>
            {student.fullName} - {student.email}
          </li>
        ))}
      </ul>
    </div>
  );
}
