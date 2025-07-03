import { useNavigate } from "react-router-dom";
import "./AdminDashboard.css";
import { useEffect, useState } from "react";

export default function AdminDashboard() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");

  useEffect(() => {
    const storedUsername = localStorage.getItem("username");
    if (!storedUsername) {
      navigate("/login");
    } else {
      setUsername(storedUsername);
    }
  }, [navigate]);

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
          <button onClick={() => navigate("/admin/manage-students")}>Manage Students</button>
          <button onClick={() => navigate("/admin/manage-teachers")}>Manage Teachers</button>
          <button onClick={() => navigate("/admin/assign-role")}>Assign Role & Identifier</button>
          <button onClick={() => navigate("/admin/create-course")}>Create Course</button>
          <button onClick={() => navigate("/admin/assign-students")}>Assign Students to Courses</button>
        </nav>
        <button className="logout-btn" onClick={handleLogout}>Logout</button>
      </aside>

      <main className="dashboard-content">
        <h1>Welcome back, {username || "Admin"} 🎓</h1>
        <p>Select an action from the sidebar to manage the system.</p>
        <div className="quick-stats">
          <div className="card">
            <h3>Total Students</h3>
            <span>--</span>
          </div>
          <div className="card">
            <h3>Total Teachers</h3>
            <span>--</span>
          </div>
          <div className="card">
            <h3>Courses</h3>
            <span>--</span>
          </div>
        </div>
      </main>
    </div>
  );
}