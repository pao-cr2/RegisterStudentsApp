import React, { useEffect, useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Typography, CircularProgress, Alert } from '@mui/material';
import axios from 'axios';

const StudentsList = () => {
    const [students, setStudents] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        axios.get('http://localhost:5000/api/students')
            .then(res => {
                setStudents(res.data);
                setLoading(false);
            })
            .catch(err => {
                console.error(err);
                setError('Error al cargar los estudiantes.');
                setLoading(false);
            });
    }, []);

    if (loading) return <CircularProgress sx={{ display: 'block', margin: '20px auto' }} />;

    if (error) return <Alert severity="error">{error}</Alert>;

    return (
        <TableContainer component={Paper}>
            <Typography variant="h5" sx={{ padding: 2 }}>Listado de Estudiantes</Typography>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Nombre Completo</TableCell>
                        <TableCell>Correo</TableCell>
                        {/* Si tienes matrícula, agrégala aquí, si no, quita esta columna */}
                        {/* <TableCell>Matrícula</TableCell> */}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {students.map(student => (
                        <TableRow key={student.id}>
                            <TableCell>{`${student.firstName} ${student.lastName}`}</TableCell>
                            <TableCell>{student.email}</TableCell>
                            {/* <TableCell>{student.enrollmentNumber}</TableCell> */}
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default StudentsList;
