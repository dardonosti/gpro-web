import './index.css';
import React, { Component } from 'react';
import { Layout, Menu, Icon, Button, Tag, Dropdown } from 'antd';
import { Route, Switch } from 'react-router-dom';
import { isMobile } from '../../utils';
import { Component404 } from '../../globalComponents';
import { Clientes, Empleados, Proyectos, Usuarios, Tareas, PanelEmpleado, Liquidacion } from './components';

const { Content, Sider, Header } = Layout;

const menuAdmin = [{
  group: 'Gestión',
  items: [{
    path: '/clientesView',
    label: 'Clientes',
    icon: 'solution'
  }, {
    path:'/empleadosView',
    label: 'Empleados',
    icon: 'team'
  }, {
    path: '/usuariosView',
    label: 'Usuarios',
    icon: 'user'
  }]
}, {
  group: 'Liquidación ',
  items: [{
    path: '/liquidaciones',
    label: 'Liquidación',
    icon: 'solution'
  }]
}];

const menuPM = [{
  group: 'Proyectos',
  items: [{
    path: '/',
    label: 'ABM Proyectos',
    icon: 'project'
  }, {
    path: '/tareasView',
    label: 'Asignar Tareas',
    icon: 'plus-circle'
  }],
}, {
  group: 'Panel Empleado',
  items: [{
    path: '/panelEmpleado',
    label: 'Panel',
    icon: 'profile'
  }]
}];

const menuMember = [{
  group: 'Panel Empleado',
  items: [{
    path: '/panelEmpleado',
    label: 'Panel',
    icon: 'profile'
  }]
}];

class Panel extends Component {
  constructor(props) {
    super(props);

    this.state = {
      collapsed: false,
      selected: ['/'],
      currentUser: JSON.parse(localStorage.getItem('currentUser'))
    };
  }

  componentDidMount = async () => {
    this.setState({
        selected: [this.props.location.pathname]
    });
  }

  componentDidUpdate = (prevProps) => {
    if (prevProps.location.pathname !== this.props.location.pathname) {
      this.setState({
        selected: [this.props.location.pathname]
      });
    }
  }

  handleLinkClick = (route) => {
    this.props.history.push(route);
  }

  onCollapse = () => {
    this.setState({ collapsed: !this.state.collapsed });
  }

  renderSlider = () => {
    const { currentUser, collapsed } = this.state;
    let menu = menuAdmin;

    if (currentUser.idRol === 2) menu = menuPM;
    if (currentUser.idRol === 3) menu = menuMember;

    return (
      <Sider
        collapsible
        defaultCollapsed={isMobile}
        onCollapse={this.onCollapse}
        breakpoint='sd'
        collapsedWidth={!isMobile ? 80 : 0}>
        <div className='logo'>
          <p style={{ color: 'white', fontSize: '1.5em', marginTop: '15px', marginLeft: '22px' }}>GPRO</p>
        </div>

        <Menu
          mode='inline'
          theme='dark'
          selectedKeys={this.state.selected}>
          {
            menu.map((item, index) => {
              if (item.group) {
                return (
                  <Menu.ItemGroup key={`group-${index}`} title={!collapsed && item.group}>
                    {
                      item.items.map((element) =>
                        <Menu.Item
                          key={element.path}
                          onClick={() => this.handleLinkClick(element.path)}>
                          <Icon type={element.icon} />
                          <span>{element.label}</span>
                        </Menu.Item>
                      )
                    }
                  </Menu.ItemGroup>
                );
              } else {
                return (
                  <Menu.Item
                    key={item.path}
                    onClick={() => this.handleLinkClick(item.path)}>
                    <Icon type={item.icon} />
                    <span>{item.label}</span>
                  </Menu.Item>
                )
              }
            })
          }
        </Menu>
      </Sider>
    );
  }

  renderHeader = () => {
    const { currentUser } = this.state;
    return (
      <Header align='right' style={{ paddingRight: '15px' }}>
        <Tag color='geekblue' hidden={isMobile}><Icon type='user' /> Usuario: {currentUser.username}</Tag>
        <Tag color='geekblue' style={{ marginRight: '25px' }} hidden={isMobile}><Icon type='tag' /> Rol: {currentUser.rol}</Tag>
        <Button icon='logout' onClick={this.logOut} breakpoint='sm' hidden={isMobile}>
          Cerrar sesión
        </Button>
        <div hidden={!isMobile}>
          <Dropdown overlay={this.menuMovil} trigger={['click']}>
            <Button type='primary' className='ant-dropdown-link' shape='circle' icon='user' onClick={this.logOut} />
          </Dropdown>
        </div>
      </Header>
    );
  }

  renderContent = () => {
    return (
      <Content className='panel--content'>
        <Switch>
          <Route exact path='/' component={Proyectos} />
          <Route exact path='/tareasView' component={Tareas} />
          <Route exact path='/clientesView' component={Clientes} />
          <Route exact path='/empleadosView' component={Empleados} />
          <Route exact path='/usuariosView' component={Usuarios} />
          <Route exact path='/panelEmpleado' component={PanelEmpleado} />
          <Route exact path='/liquidaciones' component={Liquidacion} />
          <Route component={Component404} />
        </Switch>
      </Content>
    );
  }

  menuMovil = () => {
    const { currentUser } = this.state;
    return (
      <Menu>
        <Menu.ItemGroup key='0'>
          <Tag color='geekblue'><Icon type='user' /> {currentUser.username}</Tag>
        </Menu.ItemGroup>
        <Menu.ItemGroup key='1'>
          <Tag color='geekblue' style={{ marginBottom: '10px' }}><Icon type='tag' /> {currentUser.role}</Tag>
        </Menu.ItemGroup>
        <Menu.Divider />
        <Menu.Item key='3' onClick={this.logOut}>Cerrar sesión</Menu.Item>
      </Menu>
    );
  }

  render() {
    return (
      <Layout className='panel'>
        {this.renderSlider()}
        <Layout>
          {this.renderHeader()}
          {this.renderContent()}
        </Layout>
      </Layout>
    );
  }

  logOut = () => {
    localStorage.clear();
    window.location.reload();
  }
}

export default Panel;