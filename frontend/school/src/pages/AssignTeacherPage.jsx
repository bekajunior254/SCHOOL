import { useEffect, useState } from "react";
//import axios from "axios";
import "./AssignTeacherPage.css";

export default function AssignTeacherPage() {
  const [courses, setCourses] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [selectedCourse, setSelectedCourse] = useState("");
  const [selectedTeacher, setSelectedTeacher] = useState("");
  const [message, setMessage] = useState("");
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [courseRes, teacherRes] = await Promise.all([
          axios.get("http://localhost:5000/api/courses", {
            headers: { Authorization: `Bearer ${token}` },
          }),
          axios.get("http://localhost:5000/api/admin/teachers", {
            headers: { Authorization: `Bearer ${token}` },
          }),
        ]);
        setCourses(courseRes.data);
        setTeachers(teacherRes.data);
      } catch (err) {
        console.error("Failed to fetch data", err);
      }
    };

    fetchData();
  }, []);

  const handleAssign = async (e) => {
    e.preventDefault();
    try {
      await axios.post(
        `http://localhost:5000/api/courses/${selectedCourse}/assign-teacher`,
        { teacherId: selectedTeacher },
        { headers: { Authorization: `Bearer ${token}` } }
      );
      setMessage("Teacher assigned successfully!");
    } catch (err) {
      console.error("Failed to assign teacher", err);
      setMessage(" Failed to assign teacher. Try again.");
    }
  };

  return (
    <div className="assign-teacher-page">
      <h2>Assign Teacher to Course</h2>

      <form onSubmit={handleAssign} className="assign-form">
        <label>Course:</label>
        <select
          value={selectedCourse}
          onChange={(e) => setSelectedCourse(e.target.value)}
          required
        >
          <option value="">-- Select Course --</option>
          {courses.map((course) => (
            <option key={course.id} value={course.id}>
              {course.name}
            </option>
          ))}
        </select>

        <label>Teacher:</label>
        <select
          value={selectedTeacher}
          onChange={(e) => setSelectedTeacher(e.target.value)}
          required
        >
          <option value="">-- Select Teacher --</option>
          {teachers.map((teacher) => (
            <option key={teacher.id} value={teacher.id}>
              {teacher.fullName}
            </option>
          ))}
        </select>

        <button type="submit">Assign</button>
      </form>

      {message && <p className="message">{message}</p>}
    </div>
  );
}