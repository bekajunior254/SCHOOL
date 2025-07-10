import { useEffect, useState } from "react";
import axios from "axios";
import "./StudentDashboard.css";

export default function StudentDashboard() {
  const [profile, setProfile] = useState({});
  const [enrolledCourses, setEnrolledCourses] = useState([]);
  const [availableCourses, setAvailableCourses] = useState([]);
  const [grades, setGrades] = useState([]);
  const [attendance, setAttendance] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem("token");

    const fetchAll = async () => {
      try {
        const config = { headers: { Authorization: `Bearer ${token}` } };

        const [profileRes, enrolledRes, availableRes, gradesRes, attendanceRes] = await Promise.all([
          axios.get("http://localhost:5000/api/student/profile", config),
          axios.get("http://localhost:5000/api/student/enrolled-courses", config),
          axios.get("http://localhost:5000/api/student/available-courses", config),
          axios.get("http://localhost:5000/api/student/grades", config),
          axios.get("http://localhost:5000/api/student/attendance", config)
        ]);

        setProfile(profileRes.data);
        setEnrolledCourses(enrolledRes.data);
        setAvailableCourses(availableRes.data);
        setGrades(gradesRes.data);
        setAttendance(attendanceRes.data);
      } catch (err) {
        console.error("Failed to load student dashboard data", err);
      }
    };

    fetchAll();
  }, []);

  return (
    <div className="student-dashboard">
      <h1>Welcome, {profile.firstName} {profile.lastName}</h1>

      <section>
        <h2>My Profile</h2>
        <p><strong>Admission No:</strong> {profile.admissionNumber}</p>
        <p><strong>Email:</strong> {profile.email}</p>
        <p><strong>Program:</strong> {profile.program}</p>
        <p><strong>Year:</strong> {profile.year}</p>
      </section>

      <section>
        <h2>Enrolled Courses</h2>
        <ul>
          {enrolledCourses.map(course => (
            <li key={course.id}>{course.name} ({course.code})</li>
          ))}
        </ul>
      </section>

      <section>
        <h2>Available Courses</h2>
        <ul>
          {availableCourses.map(course => (
            <li key={course.id}>{course.name} ({course.code})</li>
          ))}
        </ul>
      </section>

      <section>
        <h2>My Grades</h2>
        <table>
          <thead>
            <tr>
              <th>Course</th>
              <th>Grade</th>
            </tr>
          </thead>
          <tbody>
            {grades.map(g => (
              <tr key={g.courseId}>
                <td>{g.courseName}</td>
                <td>{g.grade}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section>
        <h2>Attendance Records</h2>
        <table>
          <thead>
            <tr>
              <th>Course</th>
              <th>Date</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {attendance.map(a => (
              <tr key={a.id}>
                <td>{a.courseName}</td>
                <td>{new Date(a.date).toLocaleDateString()}</td>
                <td>{a.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </div>
  );
}
