import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Register from "./pages/Register.jsx";
import Login from "./pages/Login.jsx";
import AdminDashboard from "./pages/AdminDashboard.jsx";
import AssignRolePage from "./pages/AssignRolePage.jsx";
import ManageStudents from "./pages/ManageStudents.jsx";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Register />} />
        <Route path="/login" element={<Login />} />
       <Route path="/admin" element={<AdminDashboard />} />
       <Route path="/admin/assign-role" element={<AssignRolePage />} />
       <Route path="/admin/manage-students" element={<ManageStudents />} />
 
      </Routes>
    </Router>
  );
}

export default App;

