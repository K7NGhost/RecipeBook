import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import './App.css';
import Home from './pages/Home';
import AddRecipe from './pages/AddRecipe';
import EditRecipe from './pages/EditRecipe';
import ViewRecipe from './pages/ViewRecipe';

function App() {
  return (
    <BrowserRouter>
      <div className="topbar">
        <nav>
          <Link to="/">Home</Link>
          <Link to="/add">Add Recipe</Link>
        </nav>
      </div>

      <main className="container">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/add" element={<AddRecipe />} />
          <Route path="/edit/:id" element={<EditRecipe />} />
          <Route path="/view/:id" element={<ViewRecipe />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}

export default App;
