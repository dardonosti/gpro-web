import './index.css';
import React, { Component } from 'react';
import { Table, Button, message, Divider } from 'antd';
import { Modal } from './components';
import { getHeader } from '../../../../utils';
import axios from 'axios';

class ProyectosView extends Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: false,
      loading: false,
      creating: false,
      proyectos: []
    };
  }

  componentDidMount() {
    this.getProyectos();
  }

  render() {
    const { visible, loading, proyectos, creating } = this.state;

    const columns = [
      {
        title: 'ID',
        dataIndex: 'idProyecto',
        key: 'idProyecto'
      },
      {
          title: 'Título',
          dataIndex: 'tituloProyecto',
          key: 'titulo'
      },
      {
        title: 'Descripción',
        dataIndex: 'descripcionProyecto',
        key: 'descripcion'
      },
      {
        title: 'Ver info',
        key: 'info',
        render: item => {
          return (
            <Button
              onClick={() => this.proyectInfo(item)}
              >
              Ver
            </Button>
          );
        }
      }
    ];

    return(
      <div>
        <Button 
          type='primary'
          icon='plus'
          onClick={this.handleModal}>
          Crear Proyecto
        </Button>

        <Divider />
        
        <Table 
          columns={columns} 
          pagination={{ pageSize: 5 }}
          dataSource={proyectos}
          loading={loading}
          scroll={{ x: true }}
          rowKey='idProyecto'
          bordered
          locale={{ emptyText: "No hay proyectos" }} />

        <Modal 
          visible={visible}
          handleModal={this.handleModal}
          creating={creating}
          crearProyecto={this.crearProyecto} />
      </div>
    );
  }

  getProyectos = async () => {
    try {
      this.setState({ loading: true });
      const res = await axios.get('http://localhost:60932/proyectos/', getHeader());
  
      this.setState({ proyectos: res.data });
    } catch (error) {
      let messageError = 'Hubo un error';
    
      if (error.response) {
        messageError = error.response.data.message || 'Hubo un error';
      }

      message.error(messageError);
    }

    this.setState({ loading: false });
  }

  handleModal = () => {
    this.setState({ 
      visible: !this.state.visible
    });
  }

  proyectInfo = (proyecto) => {}

  crearProyecto = async (form) => {
    const { clienteId, tituloProyecto, descripcionProyecto } = form;

    if (!clienteId || !tituloProyecto || !descripcionProyecto) {
      return message.error('Debe completar todos los datos');
    }

    try {
      this.setState({ creating: true });
      const res = await axios.post('http://localhost:60932/proyectos/',
        {
          ...form,
          estadoProyecto: "vigente"
        },
        getHeader());
      
      if (res.data) {
        message.success('Poyecto creado con exito');
        this.handleModal();
        this.getProyectos();
      }
    } catch (error) {
      console.log('error creando: ', error)
    }
    this.setState({ creating: false });
  }
}

export default ProyectosView;