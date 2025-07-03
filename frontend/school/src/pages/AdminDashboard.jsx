import { useNavigate } from "react-router-dom";
import { useEffect, useState, useCallback } from "react";
import axios from "axios";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [stats, setStats] = useState({ students: 0, teachers: 0, courses: 0 });

  const refreshStats = useCallback(async () => {
    try {
      const token = localStorage.getItem("token");
      const res = await axios.get("http://localhost:5000/api/admin/stats", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setStats(res.data);
    } catch (err) {
      console.error("Failed to load dashboard stats", err);
    }
  }, []);

  useEffect(() => {
    const storedUsername = localStorage.getItem("username");
    if (!storedUsername) {
      navigate("/login");
    } else {
      setUsername(storedUsername);
    }

    refreshStats();
  }, [navigate, refreshStats]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("role");
    navigate("/login");
  };

  return (
    <div className="admin-dashboard">
      <aside className="sidebar">
        <h2>Admin Panel</h2>
        <nav>
          <button onClick={() => navigate("/admin/manage-users")}>Manage Users</button>
          <button onClick={() => navigate("/admin/manage-students", { state: { refreshStats } })}>
            Manage Students
          </button>
          <button onClick={() => navigate("/admin/manage-teachers", { state: { refreshStats } })}>
            Manage Teachers
          </button>
          <button onClick={() => navigate("/admin/assign-role")}>Assign Role & Identifier</button>
          <button onClick={() => navigate("/admin/create-course")}>Create Course</button>
          <button onClick={() => navigate("/admin/enroll-students")}>Enroll Students to Courses</button>
          <button onClick={() => navigate("/admin/assign-teachers")}>Assign Courses to Teachers</button>
        </nav>
        <button className="logout-btn" onClick={handleLogout}>Logout</button>
      </aside>

      <main className="dashboard-content">
        <h1>Welcome back, {username || "Admin"} 🎓</h1>
        <p>Select an action from the sidebar to manage the system.</p>

        <button onClick={refreshStats} className="refresh-btn">🔄 Refresh Stats</button>

        <div className="quick-stats">
          <div className="card">
            <h3>Total Students</h3>
            <span>{stats.students}</span>
          </div>
          <div className="card">
            <h3>Total Teachers</h3>
            <span>{stats.teachers}</span>
          </div>
          <div className="card">
            <h3>Courses</h3>
            <span>{stats.courses}</span>
          </div>
        </div>
      </main>
    </div>
  );
}
