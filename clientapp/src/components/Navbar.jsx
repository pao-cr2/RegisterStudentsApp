import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
    const { user, logout } = useAuth();

    if (!user) return null;

    return (
        <nav>
            <Link to="/">Inicio</Link>
            {user.role === 'Admin' && (
                <>
                    <Link to="/students">Estudiantes</Link>
                    <Link to="/teachers">Profesores</Link>
                    <Link to="/subjects">Materias</Link>
                    <Link to="/enrollments">Inscripciones</Link>
                </>
            )}
            {user.role === 'Student' && (
                <>
                    <Link to="/enrollments">Mis Inscripciones</Link>
                    {/* Puedes a�adir m�s links espec�ficos */}
                </>
            )}
            <button onClick={logout}>Cerrar sesi�n</button>
        </nav>
    );
};

export default Navbar;
